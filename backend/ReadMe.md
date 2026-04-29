# HotelLakeview

HotelLakeview on hotellin varausjärjestelmä, joka koostuu backend API:sta ja modernista frontend-sovelluksesta.

Järjestelmä on toteutettu käyttäen Clean Architecturea ja moderneja web-teknologioita.

---

## Projektin tavoite

Projektin tavoitteena on tarjota skaalautuva ja laajennettava ratkaisu, joka tukee:

- asiakashallintaa
- huonehallintaa
- varauksia ja saatavuushakua
- hinnoittelulogiikkaa (sesongit)
- raportointia ja analytiikkaa
- autentikointia ja rooleja
- selainpohjaista käyttöliittymää

---

## Teknologiat

### Backend

- .NET 10 Web API
- Entity Framework Core
- PostgreSQL (Npgsql)
- Clean Architecture
- CQRS
- MediatR
- FluentValidation
- Result Pattern

### Frontend

- Next.js (App Router)
- TypeScript
- React
- Tailwind CSS

### Pilvi ja DevOps

- Azure App Service
- Azure Blob Storage (kuvat)
- GitHub Actions (CI/CD)

---

## Arkkitehtuuri

```text

Frontend (Next.js)
        ↓
API (Controllers)
        ↓
Application (CQRS + MediatR)
        ↓
Domain (Business logic)
        ↓
Infrastructure (EF Core, PostgreSQL, Azure)
## Backend

### Domain

Sisältää liiketoiminnan ydinkäsitteet ja säännöt:

- Customer
- Room
- Reservation
- RoomImage
- User

#### Enumit

- RoomType
- ReservationStatus
- UserRole

---

### Application

Sisältää:

- commandit ja queryt
- handlerit
- DTO:t
- validointi
- Result Pattern
- pagination

---

### Infrastructure

Sisältää:

- EF Core DbContext
- PostgreSQL
- repositoryt
- seed data

---

### API

- vastaanottaa HTTP-pyynnöt
- käyttää MediatR:ia
- palauttaa tulokset

---

## Frontend

Frontend on toteutettu Next.js:llä ja tarjoaa käyttöliittymän hotellin hallintaan.

### Keskeiset näkymät

- Etusivu
- Huoneiden selaus
- Varauskalenteri
- Varausten hallinta
- Admin-näkymä

---

### Toiminnallisuudet

- vapaat huoneet aikavälille
- varauksen luonti käyttöliittymästä
- varauksen muokkaus ja peruminen
- kalenterinäkymä varaustilanteelle
- käyttäjähallinta admin-paneelissa

#### Raportit

- revenue
- käyttöaste
- suosituimmat huoneet

---

### Admin-näkymä

Admin-paneelissa voidaan:

- tarkastella raportteja
- hallita käyttäjiä
- tarkastella asiakkaita ja varauksia
- nähdä varaukset asiakaskohtaisesti
- järjestää varaukset aikajärjestykseen

---

## Autentikointi

- JWT-pohjainen kirjautuminen
- AuthGuard suojaa näkymät
- roolipohjainen pääsynhallinta

---

## Pyyntöjen kulku

1. Käyttäjä tekee toiminnon frontendissä
2. Frontend kutsuu API:a
3. API ohjaa pyynnön MediatR:lle
4. Handler käsittelee logiikan
5. Tulos palautetaan frontendille
6. UI päivittyy

---

## Toteutetut ominaisuudet

### Asiakashallinta

- luonti, haku, päivitys
- hakutoiminto
- deaktivointi

---

### Huonehallinta

- CRUD
- huonetyypit
- kapasiteetti ja hinnoittelu

---

### Varausjärjestelmä

- luonti, muokkaus, peruminen
- päällekkäisten varausten esto
- saatavuushaku

---

### Hinnoittelu

- automaattinen laskenta
- sesonkihinnoittelu

---

### Raportointi

- käyttöaste
- revenue
- varausyhteenveto

---

### Tietoturva

- käyttäjät ja roolit
- JWT
- suojatut endpointit

---

## Testaus

- yksikkötestit liiketoimintalogiikalle
- handler-testit
- virhetilanteiden testaus

---

## CI/CD

- GitHub Actions pipeline
- automaattinen build ja deploy Azureen

---

## Käynnistys lokaalisti

### Backend

dotnet restore

dotnet ef database update --project src/HotelLakeview.Infrastructure --startup-project src/HotelLakeview.Api

dotnet run --project src/HotelLakeview.Api

### Frontend

cd frontend

npm install

npm run dev

Frontend löytyy:
http://localhost:3000

## API

### Backend
http://localhost:5268

### Health check
http://localhost:5268/health

### Swagger
http://localhost:5268/swagger

## Jatkokehitys
- online-varaus asiakkaille
- maksujärjestelmä
- caching (Redis)
- reaaliaikainen data
- laajempi analytiikkas