cd ..\Web\
set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://localhost:80;http://axunonb:80
dotnet TournamentCalendar.dll
pause

