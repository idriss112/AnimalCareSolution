#  Système de Gestion de Clinique Vétérinaire Animal Care

Une application web full-stack complète pour la gestion des opérations de clinique vétérinaire, développée avec ASP.NET Core MVC et Entity Framework Core.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-blue)](https://docs.microsoft.com/fr-fr/dotnet/csharp/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

##  Table des Matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Stack Technique](#stack-technique)
- [Architecture](#architecture)
- [Démarrage](#démarrage)
- [Schéma de Base de Données](#schéma-de-base-de-données)
- [Rôles Utilisateurs](#rôles-utilisateurs)
- [Points d'API](#points-dapi)
- [Sécurité](#sécurité)
- [Contribution](#contribution)
- [Licence](#licence)

## Aperçu

Animal Care Clinic est un système moderne de gestion vétérinaire basé sur les rôles, conçu pour rationaliser les opérations de la clinique. L'application fournit des interfaces distinctes pour les administrateurs, les vétérinaires et les réceptionnistes, permettant une gestion efficace des rendez-vous, des dossiers patients et des horaires du personnel.

### Points Clés

- **Système d'authentification multi-rôles** avec trois types d'utilisateurs distincts
- **Gestion complète des rendez-vous** avec suivi du statut
- **Système de planification des vétérinaires** avec application des règles métier
- **Validation des données en temps réel** et retour utilisateur
- **Design responsive** optimisé pour ordinateur et mobile
- **Rapports professionnels** avec visualisation des données

##  Fonctionnalités

###  Fonctionnalités Administrateur
- **Gestion du Personnel**
  - Créer, modifier et gérer les vétérinaires
  - Créer, modifier et gérer les réceptionnistes
  - Activer/désactiver les comptes utilisateurs
  - Assigner des spécialités aux vétérinaires

- **Gestion des Horaires**
  - Créer des horaires de vétérinaires (minimum 3 jours requis)
  - Édition en masse avec sélection multi-jours
  - Heures de travail fixes (8h - 16h)
  - Voir et gérer tous les horaires

- **Rapports & Analytiques**
  - Rapports mensuels des rendez-vous
  - Analyse du taux d'annulation
  - Métriques de performance par vétérinaire
  - Graphiques interactifs avec Chart.js

- **Vue d'Ensemble du Système**
  - Tableau de bord avec statistiques clés
  - Actions rapides pour les tâches courantes
  - Mises à jour des données en temps réel

###  Fonctionnalités Vétérinaire
- **Gestion des Horaires**
  - Voir l'horaire hebdomadaire personnel
  - Voir les jours et heures de travail assignés

- **Gestion des Rendez-vous**
  - Voir les rendez-vous assignés
  - Mettre à jour le statut des rendez-vous
  - Accéder à l'historique médical des patients

- **Gestion du Profil**
  - Mettre à jour les informations personnelles
  - Changer le mot de passe avec vérification
  - Voir les spécialités

###  Fonctionnalités Réceptionniste
- **Réservation de Rendez-vous**
  - Planifier des rendez-vous avec vétérinaires disponibles
  - Voir l'horaire du jour
  - Gérer le statut des rendez-vous

- **Gestion des Clients**
  - Enregistrer de nouveaux propriétaires d'animaux
  - Mettre à jour les informations des propriétaires
  - Rechercher et filtrer les propriétaires

- **Dossiers Patients**
  - Ajouter de nouveaux animaux
  - Mettre à jour les informations des animaux
  - Suivre l'historique médical

###  Authentification & Autorisation
- **Système d'Auto-inscription**
  - Inscription basée sur les rôles (Vétérinaire/Réceptionniste)
  - Vérification email contre les enregistrements de base de données
  - Liaison automatique des comptes aux profils du personnel

- **Gestion du Profil**
  - Vérification du mot de passe en temps réel
  - Champs verrouillés jusqu'à authentification
  - Synchronisation automatique avec les dossiers du personnel

- **Fonctionnalités de Sécurité**
  - Blocage de connexion pour utilisateurs inactifs
  - Contrôle d'accès basé sur les rôles (RBAC)
  - Protection par jeton anti-contrefaçon
  - Exigences de force du mot de passe

##  Stack Technique

### Backend
- **Framework:** ASP.NET Core MVC 8.0
- **Langage:** C# 12
- **ORM:** Entity Framework Core 8.0
- **Base de Données:** SQL Server
- **Authentification:** ASP.NET Core Identity
- **Journalisation:** ILogger (intégré)

### Frontend
- **Moteur de Templates:** Razor Views
- **Framework CSS:** Bootstrap 5.3
- **Icônes:** Bootstrap Icons
- **JavaScript:** jQuery 3.6
- **Validation:** jQuery Validation & Unobtrusive Validation
- **Graphiques:** Chart.js

### Architecture & Patterns
- **Pattern MVC:** Modèle-Vue-Contrôleur
- **Pattern Repository:** Abstraction d'accès aux données
- **Injection de Dépendances:** DI ASP.NET Core intégré
- **Async/Await:** Opérations non bloquantes
- **Principes SOLID:** Code propre et maintenable

##  Architecture

```
AnimalCare/
├── Controllers/
│   ├── AdminController.cs          # Tableau de bord admin & rapports
│   ├── AccountController.cs        # Gestion du profil utilisateur
│   ├── AppointmentsController.cs   # CRUD rendez-vous
│   ├── OwnersController.cs         # Gestion propriétaires
│   ├── AnimalsController.cs        # Dossiers patients
│   ├── VeterinariansController.cs  # Gestion vétérinaires
│   ├── ReceptionistsController.cs  # Gestion réceptionnistes
│   └── VetSchedulesController.cs   # Gestion horaires
│
├── Models/
│   ├── ApplicationUser.cs          # Utilisateur Identity étendu
│   ├── Owner.cs                    # Entité propriétaire
│   ├── Animal.cs                   # Entité patient
│   ├── Veterinarian.cs             # Entité vétérinaire
│   ├── Receptionist.cs             # Entité réceptionniste
│   ├── Appointment.cs              # Entité rendez-vous
│   ├── VetSchedule.cs              # Entité horaire
│   └── VetSpecialty.cs             # Entité spécialité
│
├── ViewModels/
│   ├── MonthlyReportViewModel.cs   # Structure données rapport
│   ├── CreateVetScheduleViewModel.cs
│   └── EditVetScheduleViewModel.cs
│
├── Data/
│   ├── AnimalCareDbContext.cs      # Contexte EF Core
│   └── Migrations/                 # Migrations base de données
│
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml          # Layout principal
│   ├── Home/
│   ├── Admin/
│   ├── Appointments/
│   ├── Owners/
│   ├── Animals/
│   ├── Veterinarians/
│   ├── Receptionists/
│   └── VetSchedules/
│
└── Areas/
    └── Identity/
        └── Pages/
            └── Account/            # Login, Register, Profile
```

##  Démarrage

### Prérequis

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Installation

1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/idriss112/AnimalCareSolution.git
   cd animal-care-clinic
   ```

2. **Mettre à jour la chaîne de connexion**
   
   Éditer `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnimalCareDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Restaurer les packages NuGet**
   ```bash
   dotnet restore
   ```

4. **Installer les outils Entity Framework Core** (si pas déjà installé)
   ```bash
   dotnet tool install --global dotnet-ef
   ```

5. **Créer et appliquer les migrations de base de données**
   
   **Important:** Vous devez créer la base de données avant d'exécuter l'application.
   
   ```bash
   # Créer la migration initiale
   dotnet ef migrations add InitialCreate
   
   # Appliquer la migration pour créer la base de données
   dotnet ef database update
   ```
   
   **Alternative avec Package Manager Console (Visual Studio):**
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```

6. **Vérifier la création de la base de données**
   
   Ouvrir SQL Server Management Studio (SSMS) et vérifier que la base de données `AnimalCareDb` a été créée avec toutes les tables.

7. **Exécuter l'application**
   ```bash
   dotnet run
   ```
   
   Ou appuyer sur `F5` dans Visual Studio

8. **Accéder à l'application**
   
   Ouvrir votre navigateur et aller à `https://localhost:7056`

### Configuration Initiale

Après avoir exécuté l'application pour la première fois, vous devrez créer les données initiales:

1. **Créer l'utilisateur Admin** (via SQL ou seeding)
   ```sql
   -- L'utilisateur admin doit être créé en premier avec les rôles appropriés
   ```

2. **Ajouter des Vétérinaires**
   - Se connecter en tant qu'Admin
   - Naviguer vers Vétérinaires → Ajouter un Vétérinaire
   - Remplir les détails du vétérinaire

3. **Créer des Horaires de Vétérinaires**
   - Naviguer vers Horaires → Créer un Horaire
   - Sélectionner le vétérinaire et les jours de travail (minimum 3 jours)

4. **Ajouter des Réceptionnistes**
   - Naviguer vers Réceptionnistes → Ajouter un Réceptionniste
   - Remplir les détails du réceptionniste

5. **Inscription du Personnel**
   - Les vétérinaires et réceptionnistes peuvent maintenant s'inscrire à `/Identity/Account/Register`
   - Ils doivent utiliser l'email enregistré dans le système

##  Schéma de Base de Données

### Entités Principales

#### ApplicationUser
Utilisateur ASP.NET Identity étendu avec propriétés personnalisées:
- `FirstName`, `LastName`
- `VeterinarianId` (FK nullable)
- `ReceptionistId` (FK nullable)
- `IsActive` (statut du compte)
- `CreatedAt`

#### Veterinarian
- Informations personnelles (nom, email, téléphone)
- `SpecializationSummary`
- Relation Many-to-Many avec `VetSpecialties`
- Relation One-to-Many avec `Appointments`
- Relation One-to-Many avec `VetSchedules`
- Relation One-to-One avec `ApplicationUser`

#### Receptionist
- Informations personnelles (nom, email, téléphone)
- Statut `IsActive`
- Relation One-to-One avec `ApplicationUser`

#### Owner (Propriétaire)
- Informations personnelles
- Relation One-to-Many avec `Animals`

#### Animal
- Informations patient (nom, espèce, race, date de naissance)
- `Weight`, `MedicalHistory`
- Appartient à un `Owner`
- Relation One-to-Many avec `Appointments`

#### Appointment (Rendez-vous)
- `AppointmentDateTime`
- `Status` (Scheduled, Completed, Cancelled, NoShow)
- `Reason`, `Notes`
- Références `Owner`, `Animal`, `Veterinarian`

#### VetSchedule (Horaire)
- `DayOfWeek` (enum)
- `StartTime`, `EndTime` (fixes: 8h - 16h)
- `IsActive`
- Appartient à un `Veterinarian`

#### VetSpecialty (Spécialité)
- `Name`, `Description`
- Relation Many-to-Many avec `Veterinarians`

### Relations

```
Owner (1) ─────────< (N) Animal
Animal (1) ─────────< (N) Appointment
Veterinarian (1) ───< (N) Appointment
Veterinarian (1) ───< (N) VetSchedule
Veterinarian (N) ───< (N) VetSpecialty
ApplicationUser (1) ─ (0..1) Veterinarian
ApplicationUser (1) ─ (0..1) Receptionist
```

##  Rôles Utilisateurs

### Hiérarchie des Rôles

| Rôle | Niveau d'Accès | Permissions Clés |
|------|----------------|------------------|
| **Admin** | Accès Complet Système | Gérer tous les utilisateurs, horaires, voir rapports |
| **Vétérinaire** | Accès Limité | Voir son propre horaire, gérer rendez-vous assignés |
| **Réceptionniste** | Opérations Clients | Réserver rendez-vous, gérer propriétaires/animaux |

### Comptes de Test par Défaut

Après la configuration initiale, créer ces comptes de test:

```
Admin:
Email: admin@animalcare.com
Mot de passe: Admin123!

Vétérinaire:
Email: vet@animalcare.com
Mot de passe: Vet123!

Réceptionniste:
Email: receptionist@animalcare.com
Mot de passe: Receptionist123!
```

##  Sécurité

### Authentification
- **ASP.NET Core Identity** pour la gestion des utilisateurs
- **Hachage de mot de passe** utilisant PBKDF2
- **Nom d'utilisateur basé sur email** pour faciliter la connexion
- **Confirmation de compte** par email (optionnel)

### Autorisation
- **Contrôle d'accès basé sur les rôles (RBAC)**
- **Attributs d'autorisation au niveau méthode**
- **Politiques d'autorisation personnalisées**
- **Jetons anti-contrefaçon** sur tous les formulaires

### Protection des Données
- **Prévention injection SQL** via requêtes paramétrées (EF Core)
- **Protection XSS** via encodage Razor
- **Protection CSRF** via jetons anti-contrefaçon
- **Application HTTPS** en production

### Règles Métier
- **Blocage utilisateur inactif** - Empêche connexion pour comptes désactivés
- **Vérification mot de passe temps réel** - Valide mot de passe actuel avant changements
- **Synchronisation compte** - Désactive automatiquement utilisateur lié quand personnel désactivé
- **Contraintes horaire** - Applique minimum 3 jours de travail requis

##  Tests

### Liste de Vérification Tests Manuels

**Authentification:**
- [ ] Admin peut se connecter
- [ ] Vétérinaire peut s'inscrire avec email valide
- [ ] Réceptionniste peut s'inscrire avec email valide
- [ ] Inscription email invalide est bloquée
- [ ] Connexion utilisateur inactif est bloquée
- [ ] Mise à jour profil nécessite mot de passe actuel

**Autorisation:**
- [ ] Admin peut accéder toutes pages
- [ ] Vétérinaire peut seulement accéder propre horaire et rendez-vous
- [ ] Réceptionniste peut réserver rendez-vous et gérer clients
- [ ] Accès non autorisé affiche "Accès Refusé"

**Logique Métier:**
- [ ] Horaire vétérinaire nécessite minimum 3 jours
- [ ] Heures de travail sont fixes (8h - 16h)
- [ ] Désactivation vétérinaire désactive compte utilisateur
- [ ] Statut rendez-vous se met à jour correctement
- [ ] Rapport mensuel calcule correctement

##  Améliorations Futures

### Fonctionnalités Prévues
- [ ] **Notifications Email** - Rappels et confirmations de rendez-vous
- [ ] **Intégration SMS** - Notifications par message texte
- [ ] **Réservation en Ligne** - Portail de réservation pour clients
- [ ] **Dossiers Médicaux** - Historique patient amélioré avec pièces jointes
- [ ] **Gestion Prescriptions** - Génération prescription numérique
- [ ] **Système Facturation** - Génération factures et suivi paiements
- [ ] **Gestion Inventaire** - Suivi fournitures et médicaments
- [ ] **Rapports Avancés** - Rapports revenus, statistiques patients
- [ ] **Intégration Calendrier** - Sync avec Google Calendar / Outlook
- [ ] **Application Mobile** - Applications natives iOS/Android
- [ ] **API RESTful** - API séparée pour intégrations tierces
- [ ] **Mises à Jour Temps Réel** - SignalR pour mises à jour rendez-vous live
- [ ] **Support Multi-cliniques** - Gérer plusieurs emplacements
- [ ] **Sauvegardes Automatiques** - Sauvegardes base de données planifiées

##  Contribution

Les contributions sont les bienvenues! Veuillez suivre ces étapes:

1. Forker le dépôt
2. Créer une branche feature (`git checkout -b feature/FonctionnaliteIncroyable`)
3. Commiter vos changements (`git commit -m 'Ajouter une FonctionnaliteIncroyable'`)
4. Pousser vers la branche (`git push origin feature/FonctionnaliteIncroyable`)
5. Ouvrir une Pull Request

### Directives de Style de Code
- Suivre les conventions de codage C#
- Utiliser des noms de variables et méthodes significatifs
- Ajouter commentaires documentation XML pour méthodes publiques
- Écrire du code propre et lisible (principes SOLID)
- Inclure gestion d'erreurs et journalisation

##  Licence

Ce projet est sous licence MIT - voir le fichier [LICENSE](LICENSE) pour détails.

##  Auteur

**Driss Laaziri**

- LinkedIn: [idrisslzr](https://www.linkedin.com/in/idrisslzr)
- GitHub: [@idriss112](https://github.com/idriss112)
- Email: idrisslaaziri@gmail.com

##  Remerciements

- Documentation ASP.NET Core
- Documentation Entity Framework Core
- Documentation Bootstrap
- Communauté Stack Overflow
- Divers tutoriels et ressources en ligne

##  Support

Pour support, idrisslaaziri@gmail.com ou ouvrir un issue dans le dépôt GitHub.

---

**Construit avec ❤️ en utilisant ASP.NET Core MVC**
