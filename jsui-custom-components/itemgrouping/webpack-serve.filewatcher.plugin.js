const fs = require("fs");
const path = require("path");
const chokidar = require("chokidar");
const WebSocket = require('ws');

function PagesFileWatcher(server, port) {
    const socket = new WebSocket(`ws://localhost:${port}`);
    const basePath =  __dirname + "/pages";
    const destination = __dirname + "/bin";
    const watcher = chokidar.watch(__dirname + '/pages', {});

    watcher.on('error', (event) => {
        console.error(event);
    });
    watcher.on('change', (file) => {
        try {
            const relativePath = path.relative(basePath, file);
            fs.createReadStream(file).pipe(fs.createWriteStream(`${destination}/${relativePath}`));
            console.log(`Copied ${relativePath}`);
            const data = {
                type: 'broadcast',
                data: {
                    type: 'window-reload',
                    data: {}
                }
            };

            socket.send(JSON.stringify(data));
        } catch (ex) {
            console.error(`Could not handle file change`, ex);
        }
    });

    server.on('close', () => {
        watcher.close();
    });
}

module.exports = PagesFileWatcher;