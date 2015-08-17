
CALL procdump -s 2 -n 4 CESService7.exe c:\ProcDumpFiles

CALL  procdump -s 2 -n 4 CESCustomCrawlers7.exe c:\ProcDumpFiles

CALL procdump -s 2 -n 4 CESConverter7.exe c:\ProcDumpFiles

CALL taskkill /f /im CESService7.exe

CALL taskkill /f /im CESCustomCrawlers7.exe

CALL taskkill /f /im CESConverter7.exe



CALL msg * "CESService7 has just stop because it was taking too much memory(RAM), please contact Coveo and check your dump files C:\ProcDumpFiles, Link to doc: LINK"
