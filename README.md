Ovaj repozitorijum sadrži početni projekat za izradu desktop aplikacije sa pristupom bazi podataka. Projekat koristi .NET 10 i WPF (Windows Presentation Foundation) za korisnički interfejs i PostgreSQL za skladištenje podataka.

## Struktura Projekta

Projekat je organizovan u dva glavna dela:

```
CommunityHub/
├── src/
│   ├── CommunityHub.Application/ # Poslovna logika i pristup bazi
│   └── CommunityHub.Ui/          # Korisnički interfejs (WPF)
└── CommunityHub.slnx
```

### CommunityHub.Application

Projekat koji sadrži domenski model, poslovnu logiku i klase za pristup bazi podataka.

#### Database/
- **PostgresConnection.cs** - Statička klasa za kreiranje konekcije na bazu
  - Čita konekcioni string za pristup bazi podataka iz `appsettings.json`
  - Metoda `CreateConnection()` se poziva od strane repozitorijumskih klasa i vraća otvoren IDbConnection
  - Ovu klasu ne treba menjati

#### Database/Repositories/
Repozitorijumske klase koje interaguju sa bazom podatka.

- **UserDbRepository.cs** - Primer repozitorijuma za dobavljanje podataka o korisnicima.
  - `GetIdByCredentials(username, password)` - Vraća ID korisnika ili -1 ako ne postoji
  - `GetWithPosts(userId)` - Učitava korisnika sa svim njegovim objavama koristeći SQL JOIN

#### Database/Scripts/
Skripte za definisanje šeme baze podataka i početnih torki radi jednostavnijeg testiranja softvera.

- **database.sql** - SQL skripta za kreiranje tabela
  - Tabela `users` - Korisnički podaci
  - Tabela `posts` - Objave korisnika
  - **Pokrenuti pre prvog startovanja aplikacije!**

- **seed.sql** - SQL skripta za popunjavanje testnim podacima
  - 8 testnih korisnika (marko/marko123, ana/ana123, itd.)
  - 20 testnih objava raspoređenih između korisnika
  - **Pokrenuti nakon database.sql!**

#### Domain/
Model podataka i poslovne logike koja radi nad tim podacima.

- **User.cs** - Domenski model korisnika
  - Svojstva: Id, Username, Password, Name, Surname, BirthDay, Posts
  - Metoda `AddPost(post)` - Povezuje objavu sa korisnikom
  - **Obratiti pažnju:** Sva svojstva imaju `private set` za kontrolu pristupa

- **Post.cs** - Domenski model objave
  - Svojstva: Id, Title, Content, CreatedAt, User
  - **Obratiti pažnju:** `User` je referenca na vlasnika objave

#### appsettings.json
Konfiguracioni fajl sa podacima za pristup bazi.

### CommunityHub.Ui

WPF projekat koji sadrži sve prozore i korisničku interakciju.

#### App.xaml / App.xaml.cs
- Ulazna tačka aplikacije
- `StartupUri="/Views/LogInForm.xaml"` - Prvi prozor koji se otvara

#### Views/
- **LogInForm.xaml** - Forma za prijavu korisnika
- **LogInForm.xaml.cs** - Code-behind logika
  - Poziva `UserDbRepository.GetIdByCredentials()` pri kliku na dugme
  - Ako korisnik postoji, otvara `HomeWindow` i zatvara login formu
  - Ako ne postoji, prikazuje poruku o grešci

- **HomeWindow.xaml** - Početna stranica nakon prijave
- **HomeWindow.xaml.cs** - Code-behind logika
  - **Obratiti pažnju:** Prima `userId` u konstruktoru
  - Dugme "Profil" otvara ProfileWindow
  - Dugme "Odjavi se" vraća na LogInForm

- **ProfileWindow.xaml** - Profil korisnika sa objavama
  - **Obratiti pažnju:** Koristi ItemsControl sa DataTemplate za prikaz objava
- **ProfileWindow.xaml.cs** - Code-behind logika
  - Poziva `UserDbRepository.GetWithPosts()` da učita podatke
  - Povezuje Posts listu sa ItemsControl kroz ItemsSource

## Pokretanje Projekta

### Preduslovi

1. **.NET 10 SDK** instaliran
2. **PostgreSQL** instaliran i pokrenut na localhost:5432
3. **Kreirana baza podataka** sa imenom `communityhub`
4. **Definisanje šeme i početnih podataka** kroz pgAdmin alat puštanjem `database.sql` i `seed.sql` skripti.

Za korake 2-4 instaliracij PostgreSQL i upoznaj se sa pgAdmin alatom. Za instalaciju je potrebno:

1. Preuzeti instalaciju najnovije verzije sa <a href="https://www.enterprisedb.com/downloads/postgres-postgresql-downloads" target="_blank">sledećeg linka</a>, gde biraš instalaciju za svoj operativni sistem (npr. Windows x86-64).
2. Pokrenuti i kompletirati proces instalacije. Preporuka je da korisničko ime i lozinka budu `postgres`, a port `5432`.

Uz PostgresSQL bazu podataka dobijaš i **pgAdmin** aplikaciju kroz koju možeš da formiraš i sprovodiš SQL naredbe. <a href="https://youtu.be/Q3yDPIEV1R4" target="_blank"><b>Sledeći video</b></a> demonstrira kako se kreira baza podataka i tabele i izvršavaju SQL naredbe.

### Pokretanje Aplikacije

1. Otvorite `CommunityHub.slnx` u Visual Studio
2. Postavite `CommunityHub.Ui` kao StartUp project
3. Pokrenite aplikaciju (F5)
4. Prijavite se sa nekim od korisnika (npr. "ana", "ana123")

## Česte Greške i Rešenja

### Greška: "Connection refused"
- **Uzrok:** PostgreSQL nije pokrenut ili ne radi na localhost:5432
- **Rešenje:** Pokrenite PostgreSQL servis ili proverite port u appsettings.json

### Greška: "Relation 'users' does not exist"
- **Uzrok:** Database skripta nije pokrenuta
- **Rešenje:** Pokrenite `database.sql` koristeći pgAdmin

### Greška: "Invalid username or password"
- **Uzrok:** Seed skripta nije pokrenuta ili korisnički podaci ne postoje
- **Rešenje:** Pokrenite `seed.sql` koristeći pgAdmin
