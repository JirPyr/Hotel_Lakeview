HotelLakeview

HotelLakeview on hotellin varausjärjestelmän backend API, joka on toteutettu .NET Web API:na käyttäen modernia arkkitehtuuria ja hyväksi todettuja suunnittelumalleja.

Projektin tavoitteena on tarjota skaalautuva ja laajennettava backend, joka tukee:

asiakashallintaa
huonehallintaa
varauksia ja saatavuushakua
hinnoittelulogiikkaa (sesongit)
myöhemmin autentikointia ja rooleja
Teknologiat

Projektissa käytetyt keskeiset teknologiat:

.NET 10 Web API
Entity Framework Core
PostgreSQL (Npgsql)
Clean Architecture
CQRS (Command Query Responsibility Segregation)
MediatR
FluentValidation
Result Pattern
GitHub Actions (CI/CD)
Arkkitehtuuri

Projektissa käytetään Clean Architecturea, jossa vastuut on jaettu selkeästi eri kerroksiin.

Domain

Sisältää liiketoiminnan ydinkäsitteet ja säännöt.

Keskeiset entiteetit:

Customer
Room
Reservation
RoomImage
User

Enumit:

RoomType
ReservationStatus
UserRole

Domain ei sisällä tietokanta- tai HTTP-logiikkaa.

Application

Sisältää sovelluksen käyttötapaukset.

Tähän kerrokseen kuuluvat:

commandit ja queryt
handlerit (MediatR)
DTO:t
repository-rajapinnat
validointi (FluentValidation)
Result Pattern
pagination

Application ohjaa järjestelmän toimintaa, mutta ei tiedä teknisestä toteutuksesta.

Infrastructure

Sisältää tekniset toteutukset.

Tähän kuuluvat:

EF Core DbContext
PostgreSQL-yhteys
repositoryjen toteutukset
seed data

Infrastructure vastaa datan tallennuksesta ja ulkoisista riippuvuuksista.

API

API-kerros vastaanottaa HTTP-pyynnöt.

Controllerien tehtävä:

vastaanottaa pyyntö
muodostaa command tai query
lähettää se MediatR:lle
palauttaa tulos HTTP-vastauksena

Controllerit pidetään ohuina eikä niihin sijoiteta liiketoimintalogiikkaa.

Pyyntöjen kulku järjestelmässä

Järjestelmän perusvirta:

HTTP-pyyntö saapuu controllerille
Controller luo commandin tai queryn
Pyyntö lähetetään MediatR:lle
Handler käsittelee pyynnön Application-kerroksessa
Handler käyttää repository-rajapintaa
Infrastructure hakee tai tallentaa datan
Handler palauttaa Result- tai Result<T>-olion
API palauttaa HTTP-vastauksen
Tämänhetkinen tila

Projektissa on toteutettu:

PostgreSQL-tietokanta EF Coren kautta
kaikki keskeiset repositoryt EF-toteutuksella
varauslogiikka (sisältäen päällekkäisyyksien eston)
hinnoittelulogiikka (sesonkihinnoittelu)
validointi FluentValidationilla
kattavat yksikkötestit
health check endpoint (/health)
CI/CD pipeline GitHub Actionsilla
Käynnistäminen lokaalisti
Varmista että PostgreSQL on käynnissä
Päivitä connection string appsettings.json tiedostoon
Aja migrationit:
dotnet ef database update
Käynnistä sovellus:
dotnet run --project src/HotelLakeview.Api
API löytyy osoitteesta:
http://localhost:5268

Health check:

http://localhost:5268/health