# Project Alpha

Project alpha is an console application designed for retrieving a large amount of data from weather station(s).

## Usage
Download the release zip file.<br>
Find your desired weather station(s) from https://www.wunderground.com/wundermap - click on them and open the station id link.<br>
![Screenshot 2024-12-16 182937](https://github.com/user-attachments/assets/b41e314a-ba9c-4161-9619-fd04dc02e8cc)
Copy the url, eg "https://www.wunderground.com/dashboard/pws/IPRAGU542" and paste it into the `config.yaml` configuration file.<br>
Edit the other `config.yaml` properties such as which unit system you would like to export and such.<br>
Run the `ProjectAlpha.exe` and wait for it to finish.<br>
Report any issues (exceptions) to the github issues.<br>

## Reports

Report from Jiří S.
> Testování funkčnosti programu proběhlo úspěšně, kde bylo ověřeno, zda program zvládne v předem definovaném časovém rozmezí načíst relevantní data o počasí z externích stanic.<br>
> Otestováno bylo také načítání dat z několika stanic naráz a za jiných předvoleb v konfiguraci s uspokojujícími výsledky. Načtená data byla relativně dobře čitelná v tabulkovém programu, až na vyjímku špatného stylování sloupců.

## Used Sources
https://github.com/dotnet/runtime/issues/13051
