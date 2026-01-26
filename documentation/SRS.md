# Cahier des charges (SRS léger) — <Nom du projet>
**Équipe : Les sharks (Cedrick, Sasha, Jordann)  
**Date :** 2026-01-22  
**Version :** v0.1 / v1.0

---

## 1. Contexte & objectif
- **Contexte :** De nos jours, le pouvoir d'achat est en danger et en declin au moins au canada, la recherche de revenue alternatif est en vogue surtout parmi les jeunes investisseurs curieux.
- **Objectif principal :** Offrir une platforme simple, intuitive et transparente de portfolio de stocks pour les investisseurs.
- **Parties prenantes :** Investisseurs, Stockify admin.

---

## 2. Portée (Scope)
### 2.1 Inclus (IN)
IN-1 : Authentification + profil utilisateur.
IN-2 : Recherche de titres (actions/ETFs) par symbole ou nom.
IN-3 : Affichage des infos d’un titre : prix, variation, volume, graphique historique.
IN-4 : Watchlist (liste de suivi) personnalisée.
IN-5 : Portefeuille virtuel (cash + positions) et valeur totale.
IN-6 : Simulation d’ordres (achat/vente) au prix du marché (market) + historique des transactions.
IN-7 : Indicateurs simples.
IN-8 : Export des transactions/portefeuille (CSV).

### 2.2 Exclu (OUT)
OUT-1 : Produits complexes (options, futures, margin, short selling) 
OUT-2 : Gestion fiscale complète.

---

## 3. Acteurs / profils utilisateurs
- **Acteur A :** Investisseur, suivre des titres, comprendre l’évolution, completer des achats/ventes, veut une UI simple avec des résultats clairs.
- **Acteur B :** Admin , gérer utilisateurs

---

## 4. Exigences fonctionnelles (FR)
> Forme recommandée : “Le système doit…”
- **FR-1 :** Le système doit authentifier les utilisateurs grace a une page de login ou une page de creation de compte grace au courriel et un mot de passe.
- **FR-2 :** Le système doit pouvoir montrer a l'utilisateur connecter son portfolio qui contient chaque stocks qui possede, chaque stock avec sa valeur investis a date par le user.
- **FR-3 :** Le system doit inclure une barre de recherche universelle dans l'application qui permet de rechercher une stock grace a son nom que ce soit dans le portfolio personnel ou dans la liste de stock de notre platforme. Cette barre de recherche inclut une boutton qui offre different filtre a appliquer aussi.
- **FR-4 :** Le system doit avoir un bouton ou un onglet pour afficher la liste de stock totale que notre application offre, en appuyant sur une des stocks, on trouve une page similaire a celle de notre portfolio lorsqu'on click sur une de nos stock. Il est affiche la valeur actuelle de la stock, un graph montrant son evolution et une multitude d'autres informations a propos d'elle.
- **FR-5 :** Le system doit permettre la vente et achat de n'importe que stock disponible directement sur la page qui affiche les infos sur une stock particuliere. La vente de stock se fait uniquement depuis notre portfolio evidemment. En appuyant sur le bouton vente/achat cela nous amene sur une page qui nous permet de rentrer le montant specifique de stock ou la valeur precis que l'on souhaite acheter/vendre puis un bouton pour confirmer la transaction.
- **FR-6 :** Le system doit avoir un onglet qui affiche toutes les anciennes transactions effectuer sur l'application par le user et leur infos.
- **FR-7 :** Le system doit avoir une bouton pour se deconnecter de son compte qui nous amene ensuite a une page de connexion.

---

## 5. Exigences non fonctionnelles (NFR)
> Performance / sécurité / disponibilité / UX / maintenabilité…
- **NFR-1 (Performance) :** temps de chargement en dessous de 2s. Application non intensive pour les systems moins performants.
- **NFR-2 (Sécurité) :** Authentification requise au lancement de l'application et possibilite d'ajouter la verification par email ou par SMS. Code robuste qui assure un program anti-piratation.
- **NFR-3 (UX) :** UI tres simple et intuitif, parcours de l'application en 1-2 onglets
- **NFR-4 (Qualité) :** bonne integration et compatibilite avec les systems windows desktop.

---

## 6. Contraintes
- **C-1 (Technologie) :** C# .NET / probablement WPF pour un UI moderne.
- **C-2 (Plateforme) :** Desktop Windows
- **C-3 (Délai) :** Lancement du projet (8 janvier au 25 Janvier 2026), Analyse, exigences et premiers patrons (26 janvier au 22 février 2026), Raffinement architectural et conception avancée (23 février au 22 mars 2026).  Intégration, optimisation et robustesse (23 mars au 19 Avril)
- **C-4 (Outils) :** Git + gestion tâches (Trello/Jira) + Visual Studio

---

## 7. Données & règles métier (si applicable)
###7.1 Entités principales :**
-   #EP-1: User: Identifie un utilisateur
-   Attributs principaux: IdentifiantUtilisateur, NomUtilisateur, Email, Rôle(CLIENT, ADMIN), Solde, Devise, StatutDeCompte
-   #EP-2: Actions: Représente une action cotée en bourse
-   Attributs principaux: IdentifiantBoursier, NomEntreprise, Devise, Valeur
-   #EP-3: Transaction: Gestion d'une transaction
-   Attributs principaux: IdentifiantTransaction, IdentifiantUtilisateur, Type(Achat,Vente), IdentifiantBoursier, Quantité, Valeur, Total, Date, StatutDeTransaction
-   #EP-4: Portfolio: Totalité des actions d'un utilisateur
-   Attributs principaux: IdentifiantUtilisateur, NomUtilisateur, IdentifiantBoursier, NomEntreprise, Quantité, Valeur
###Règles métier :
 Règles de validation (RV):
-   RV-1: Un utilisateur doit être actif pour réaliser une transaction.
-   RV-2: La quantité d'une transaction doit être suppérieur à 0.
-   RV-3: Une transaction d'achat peut seulement être réaliser si le solde du compte est suppérieur à la valeur de la transaction.
-   RV-4: Une transaction de vente peut seulement être réaliser si la quantité d'action vendu est inférieur à la quantité possédée.

 Règles de calcul (RC):
-  RC-1: Le total d'une transaction correspond à: quantité * valeur.
-  RC-2: Le solde est mis à jour après chaque transaction complété.
-  RC-3: La valeur est recalculer à partir de la valeur actuel des actions.

 Règles de gestion de statut (RGS):
-  RGS-1: Une transaction peut être: EN_ATTENTE, ANNULÉ, COMPLÉTÉ, EN_COURT.
-  RGS-2: Un compte peut être: ACTIF, INNACTIF, BANNI.

 Règles de permissions (RP):
-  RP-1: Un User de type CLIENT peut uniquement consulter son propre Portfolio ou passer, annulé et consulté ses propres Transactions.
-  RP-2: Un User de type ADMIN peut consulter les informations de tout les autres comptes mais ne peut pas passer de Transaction.

---

## 8. Hypothèses & dépendances
### 8.1 Hypothèses
- H-1 : Les utilisateurs ont une connection internet.
- H-2 : Les utilisateurs ont un ordinateur.
- H-3 : Les utilisateurs utilisent le systeme d'exploitation windows 11.
- H-4 : Les utilisateurs ont un compte bancaire pour réaliser des transactions.

### 8.2 Dépendances
- D-1 : Accès a une API gratuite de marcher boursier.


---

## 9. Critères d’acceptation globaux (Definition of Done – mini)
- [ ] Fonctionnalités livrées et testées
- [ ] Tests unitaires présents
- [ ] Gestion d’erreurs minimale
- [ ] Documentation à jour (UML + ADR si requis)
