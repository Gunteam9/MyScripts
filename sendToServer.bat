if %ComputerName%==DESKTOP-VNP7NL1 goto copyDebian
if %ComputerName%== goto copyLadislus
:copyDebian
scp -r %TMP%/[my] debian@51.91.98.193:/FxServer/server-data/resources
:copyLadilsus
scp -r %TMP%/[my] ladislus@51.91.98.193:/FxServer/server-data/resources