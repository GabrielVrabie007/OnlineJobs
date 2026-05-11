# LAB 6 - Implementarea Design Pattern-urilor Comportamentale

## 1. Observer Pattern

### Locația implementării
- `Application/Observers/` - toate clasele observer și subject
- `Controllers/ApplicationController.cs` - integrare în acțiuni Apply, Withdraw, UpdateStatus

### Cum se declanșează
1. **Job Seeker**: Aplică la un job sau retrage aplicația
2. **Employer**: Schimbă statusul unei aplicații (Approve/Reject)
3. **Sistem**: Notifică automat toți observatorii înregistrați

### De ce aici
Aplicațiile pentru job-uri necesită notificări în timp real către părțile interesate. Când un job seeker aplică, employerul trebuie notificat. Când statusul unei aplicații se schimbă, job seeker-ul trebuie alertat.

### Avantaje
- **Decuplare**: Logica de notificare este separată de logica business
- **Extensibilitate**: Adăugarea unui nou tip de notificare (SMS, push notification) nu modifică codul existent
- **Consistență**: Toate notificările sunt trimise automat, fără risc de a fi uitate
- **Thread-safe**: Implementare sigură pentru acces concurent

---

## 2. Iterator Pattern

### Locația implementării
- `Application/Iterators/` - iteratori pentru aplicații, job-uri și categorii
- `Controllers/ApplicationController.cs:MyApplications()` - filtrare și sortare aplicații

### Cum se declanșează
1. **Job Seeker**: Accesează pagina "My Applications"
2. **Sistem**: Folosește FilteredApplicationIterator pentru a filtra după status
3. **Sistem**: Folosește DateOrderedApplicationIterator pentru sortare cronologică

### De ce aici
Utilizatorii trebuie să parcurgă liste mari de aplicații/job-uri cu criterii diferite de filtrare și sortare. Fiecare utilizator are nevoi diferite de vizualizare a datelor.

### Avantaje
- **Separarea responsabilităților**: Logica de traversare este separată de colecție
- **Multiple strategii**: Același set de date poate fi parcurs în moduri diferite
- **Encapsulare**: Detaliile interne ale colecției sunt ascunse
- **Uniformitate**: Interfață consistentă pentru toate tipurile de iteratori

---

## 3. Strategy Pattern

### Locația implementării

#### Salary Strategies
- `Application/Strategies/SalaryStrategies/` - calcul salariu anual pentru diferite tipuri de compensație

#### Scoring Strategies
- `Application/Strategies/ScoringStrategies/` - evaluarea candidaților
- `Controllers/ApplicationController.cs:ReceivedApplications()` - ranking aplicanți

### Cum se declanșează
1. **Employer**: Accesează lista de aplicații primite pentru un job
2. **Sistem**: Folosește ComprehensiveScoringStrategy pentru a calcula scor fiecare candidat
3. **Sistem**: Sortează candidații după scor (cel mai potrivit primul)

### De ce aici
Evaluarea candidaților este complexă și subiectivă. Diferiți angajatori pot avea criterii diferite (unii prioritizează skill-urile, alții experiența). Algoritmul de scoring trebuie să fie flexibil și interschimbabil.

### Avantaje
- **Flexibilitate**: Schimbarea algoritmului de scoring fără modificare cod client
- **Testabilitate**: Fiecare strategie poate fi testată independent
- **Claritate**: Fiecare strategie are responsabilitate clară și focalizată
- **Extensibilitate**: Adăugarea unei noi strategii nu afectează cele existente

---

## 4. Command Pattern

### Locația implementării
- `Application/Commands/` - comenzi pentru aplicații și job-uri
- `Application/Commands/CommandInvoker.cs` - executor comenzi cu undo/redo
- `Controllers/ApplicationController.cs` - toate acțiunile care modifică starea aplicațiilor

### Cum se declanșează
1. **Job Seeker**: Trimite aplicație → SubmitApplicationCommand
2. **Job Seeker**: Retrage aplicație → WithdrawApplicationCommand
3. **Employer**: Aprobă aplicație → ApproveApplicationCommand
4. **Employer**: Respinge aplicație → RejectApplicationCommand
5. **Orice utilizator**: Poate face Undo pentru ultima acțiune

### De ce aici
Acțiunile asupra aplicațiilor sunt critice și ireversibile (din perspectiva utilizatorului). Utilizatorii pot face greșeli (apăsare accidentală pe buton, schimbare de decizie) și au nevoie de posibilitatea de a anula acțiuni.

### Avantaje
- **Undo/Redo**: Posibilitatea de a anula și reface operațiuni
- **Audit trail**: Istoric complet al tuturor comenzilor executate
- **Decuplare**: Expeditorul comenzii nu cunoaște detaliile execuției
- **Macro commands**: Posibilitatea de a grupa multiple comenzi
- **Queue management**: Comenzile pot fi executate asincron sau în batch

---

## 5. Memento Pattern

### Locația implementării
- `Application/Mementos/` - memento pentru profile, job-uri și aplicații
- `Application/Mementos/ApplicationDraftManager.cs` - manager auto-save drafturi
- `Controllers/ApplicationController.cs:SaveDraft(), LoadDraft(), Apply()` - salvare/restaurare drafturi

### Cum se declanșează
1. **Job Seeker**: Începe să completeze formularul de aplicare
2. **Sistem**: Auto-save la fiecare 30 secunde (JavaScript)
3. **Job Seeker**: Închide browser-ul sau navighează în altă parte
4. **Job Seeker**: Revine la job → draft-ul este restaurat automat
5. **Job Seeker**: Trimite aplicația → draft-ul este șters

### De ce aici
Completarea formularului de aplicare necesită timp (cover letter personalizat, verificare informații). Utilizatorii pot fi întrerupți, pot avea probleme tehnice (browser crash) sau pot vrea să continue mai târziu de pe alt dispozitiv.

### Avantaje
- **Recuperare date**: Prevenirea pierderii datelor în caz de eroare sau închidere accidentală
- **Experiență îmbunătățită**: Utilizatorii pot continua de unde au rămas
- **Încapsulare**: Starea internă a obiectului este salvată fără a expune detalii
- **Versioning**: Posibilitatea de a restaura versiuni anterioare
- **Multi-device**: Draft-ul poate fi accesat de pe alte dispozitive (dacă backend-ul este persistent)

---

## Integrare Patterns

Toate cele 5 pattern-uri lucrează împreună în fluxul real al aplicației:

### Exemplu: Job Seeker aplică la un job

```
1. Memento: Încarcă draft salvat automat (dacă există)
2. Command: SubmitApplicationCommand encapsulează cererea
3. Command Invoker: Execută comanda și o adaugă în istoric (permite undo)
4. Observer: ApplicationStatusSubject notifică toți observatorii
   - EmailObserver → trimite email către employer
   - DashboardObserver → adaugă notificare în dashboard
   - AuditLogObserver → înregistrează în log
5. Memento: Șterge draft-ul după trimitere cu succes
```

### Exemplu: Employer vizualizează aplicațiile primite

```
1. Iterator: FilteredApplicationIterator filtrează după status "Submitted"
2. Strategy: ComprehensiveScoringStrategy calculează scor pentru fiecare candidat
   - SkillMatchStrategy (50% weight)
   - ExperienceStrategy (30% weight)
   - EducationStrategy (20% weight)
3. Iterator: DateOrderedIterator sortează aplicațiile
4. Sistem: Afișează candidații ranked by score
```

---

## Concluzie

Implementarea acestor pattern-uri comportamentale aduce următoarele beneficii generale:

- **Mentenabilitate**: Cod mai ușor de întreținut și modificat
- **Extensibilitate**: Funcționalități noi pot fi adăugate fără refactorizare majoră
- **Testabilitate**: Fiecare componentă poate fi testată independent
- **Separarea responsabilităților**: Fiecare clasă are un singur scop bine definit
- **Experiență utilizator îmbunătățită**: Auto-save, undo/redo, notificări automate, ranking inteligent
- **Scalabilitate**: Arhitectura suportă creșterea aplicației

Toate pattern-urile sunt integrate natural în fluxurile reale ale utilizatorilor, lucrând invizibil în fundal pentru a oferi o experiență superioară.
