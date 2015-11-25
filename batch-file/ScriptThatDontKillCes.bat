CALL procdump -ma CESService7.exe c:\ProcDumpFiles

CALL procdump -ma CESCustomCrawlers7.exe c:\ProcDumpFiles

CALL procdump -ma CESConverter7.exe c:\ProcDumpFiles



CALL msg * "Please contact Coveo support, Too much memory is used by the service.""

DEL "%~f0"

REM Line 11 is to make sure the script deletes itself after beign executed to avoid multiple Dumps 
