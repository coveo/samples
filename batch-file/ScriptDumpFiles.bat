
CALL procdump -s 2 -n 4 CESService7.exe c:\ProcDumpFiles

CALL  procdump -s 2 -n 4 CESCustomCrawlers7.exe c:\ProcDumpFiles

CALL procdump -s 2 -n 4 CESConverter7.exe c:\ProcDumpFiles

CALL taskkill /f /im CESService7.exe

CALL taskkill /f /im CESCustomCrawlers7.exe

CALL taskkill /f /im CESConverter7.exe



CALL msg * "CESService7 has been stopped because it was taking too much memory, please contact Coveo. More informations :  "https://developers.coveo.com/display/SupportKB/TroubleShooting+memory+issues+with+CESService7""


