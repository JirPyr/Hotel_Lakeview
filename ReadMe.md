# HotelLakeview - arkkitehtuurin yleiskuva

## Projektin tavoite
Tämän projektin tavoitteena on toteuttaa hotellin varausjärjestelmän backend API, jolla voidaan hallita asiakkaita, huoneita, varauksia, raportteja ja myöhemmin myös huonekuvia sekä käyttäjäoikeuksia.

Järjestelmä suunnitellaan niin, että se voidaan myöhemmin julkaista Azureen ja laajentaa tukemaan myös asiakkaiden omaa online-varausta.

---

## Miksi valitsin tämän arkkitehtuurin
Projektissa käytetään Clean Architecturea, CQRS-mallia, MediatR:ää ja Result Patternia.

Valinnan syyt:
- liiketoimintalogiikka pysyy erillään HTTP-rajapinnasta
- tietokantatoteutus voidaan vaihtaa myöhemmin ilman suuria muutoksia
- ratkaisu on helpompi testata
- rakenne tukee myöhempää Azure-julkaisua
- järjestelmää voidaan laajentaa vaiheittain

---

## Projektin kerrokset

### Domain
Domain sisältää järjestelmän ydinkäsitteet ja liiketoimintasäännöt.

Tässä projektissa Domain sisältää esimerkiksi:
- Customer
- Room
- Reservation
- RoomImage
- User
- enumit kuten RoomType, ReservationStatus ja UserRole

Domainissa ei ole tietokanta- tai HTTP-logiikkaa.

### Application
Application sisältää sovelluksen käyttötapaukset.

Tähän kerrokseen tulevat:
- commandit
- queryt
- handlerit
- DTO:t
- repository-rajapinnat
- Result Pattern
- pagination
- pipeline behaviorit

Application ohjaa mitä järjestelmä tekee, mutta ei tiedä miten data tallennetaan teknisesti.

### Infrastructure
Infrastructure sisältää tekniset toteutukset.

Tähän kerrokseen tulevat:
- repositoryjen toteutukset
- aluksi in-memory/mock-repositoryt
- myöhemmin EF Core + tietokanta
- tiedostojen tallennus
- myöhemmin mahdollinen Azure Blob Storage
- mahdollinen autentikoinnin tekninen toteutus

### API
API-kerros vastaanottaa HTTP-pyynnöt.

Controllerien tehtävä:
- vastaanottaa pyyntö
- muodostaa command tai query
- lähettää se MediatR:lle
- palauttaa tulos HTTP-vastauksena

Controllerit pidetään ohuina, eikä niihin laiteta liiketoimintalogiikkaa.

---

## Miksi aloitin in-memory-repositoryilla
Projektissa aloitetaan mock/in-memory-repositoryilla, jotta:
- arkkitehtuuri saadaan rakennettua ensin oikein
- ydintoiminnot voidaan testata ilman oikeaa tietokantaa
- toteutus voidaan pitää vaiheittaisena
- oikea tietokanta voidaan lisätä myöhemmin Infrastructure-kerrokseen ilman suurta refaktorointia

Tämä tukee myös Azure-julkaisua, koska tallennusratkaisu pysyy vaihdettavana.

---

## Pyyntöjen kulku järjestelmässä
Järjestelmän perusvirta on seuraava:

1. HTTP-pyyntö saapuu controllerille
2. Controller luo commandin tai queryn
3. Controller lähettää pyynnön MediatR:lle
4. Handler käsittelee pyynnön Application-kerroksessa
5. Handler käyttää repository-rajapintaa
6. Infrastructure toteuttaa datan haun tai tallennuksen
7. Handler palauttaa Result- tai Result<T>-olion
8. API muuntaa tuloksen HTTP-vastaukseksi

Tällä tavalla vastuut pysyvät selkeinä.

---

## Tähän mennessä tehdyt ratkaisut

### Domain
Tähän mennessä on suunniteltu domainin ydinkäsitteet:
- Customer
- Room
- Reservation
- RoomImage
- User

Lisäksi on määritelty enumit:
- RoomType
- ReservationStatus
- UserRole

Näillä mallinnetaan hotellin keskeiset liiketoimintakäsitteet.

### Application Common
Tähän mennessä on valmisteltu:
- Result
- Result<T>
- Error
- ErrorType
- PaginationRequest
- PagedResult<T>

Näiden avulla virheenkäsittely ja listaukset voidaan toteuttaa johdonmukaisesti.

---

## Jatkokehityksen suunta
Seuraavaksi toteutetaan:
1. repository-rajapinnat
2. in-memory repositoryt
3. ensimmäiset commandit ja queryt
4. controllerit
5. saatavuushaku ja varauslogiikka
6. myöhemmin oikea tietokanta
7. myöhemmin Azure-julkaisu
8. myöhemmin mahdollinen asiakkaiden online-varaus