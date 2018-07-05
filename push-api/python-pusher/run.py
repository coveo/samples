import pusher
import scraper
import parameters
import time

MESSAGES = parameters.MESSAGES()
SOURCE_STATUS = parameters.SOURCE_STATUS()

if __name__ == "__main__":
    # Run the batch
    status = pusher.Status()
    scraper = scraper.Scraper()
    start_time = time.time()
    print(MESSAGES.START_BATCH)
    status.changeSourceStatus(SOURCE_STATUS.REBUILD)
    scraper.pushAllDocs()
    status.changeSourceStatus(SOURCE_STATUS.IDLE)
    elapsed_time = round(time.time() - start_time,2)
    print(MESSAGES.END_BATCH, elapsed_time, MESSAGES.BATCH_TIME_UNIT)