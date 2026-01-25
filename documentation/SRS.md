# Cahier des charges (SRS léger) — <Nom du projet>
**Équipe : Les sharks (Cedrick, Sasha, Jordann)  
**Date :** 2026-01-22  
**Version :** <v0.1 / v1.0>

---

## 1. Contexte & objectif
- **Contexte :** De nos jours, l'economie est en danger et en declin au moins au canada, la recherche de revenue alternatif est en vogue surtout parmi les jeunes curieux investisseurs.
- **Objectif principal :** Offrir  une platforme simple, intuitive et transparente pour les investisseurs.
- **Parties prenantes :** Investisseurs , Stockify admin.

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
- **Acteur A :** <Investisseur,suivre des titres, comprendre l’évolution, tester des achats/ventes sans risque, veut une UI simple avec des résultats clairs.>
- **Acteur B :** <Admin , gérer utilisateurs>

---

## 4. Exigences fonctionnelles (FR)
> Forme recommandée : “Le système doit…”
- **FR-1 :** Le système doit authentifier les utilisateurs grace a une page de login ou une page de creation de compte grace au courriel et un mot de passe.
- **FR-2 :** Le système doit pouvoir montrer a l'utilisateur connecter son portfolio qui contient chaque stocks qui possede, chaque stock avec sa valeur investis a date par le user.
- **FR-3 :** Le system doit inclure une barre de recherche universelle dans l'application qui permet de rechercher une stock grace a son nom que ca soit dans le portfolio personnel ou dans la liste de stock de notre platforme. Cette barre de recherche inclut une boutton qui offre different filtre a appliquer aussi.
- **FR-4 :** Le system doit avoir un bouton ou un onglet pour afficher la liste de stock totale que notre application offre, en appuyant sur une des stocks, on trouve une page similaire a celle de notre portfolio lorsqu'on click sur une de nos stock. Il est affiche la valeur actuelle de la stock, un graph montrant son evolution et une multitude d'autres informations a propos d'elle.
- **FR-5 :** Le system doit permettre la vente et achat de n'importe que stock disponible directement sur la page qui affiche les infos sur une stock particuliere. La vente de stock se fait uniquement depuis notre portfolio evidemment. En appuyant sur le bouton vente/achat cela nous amene sur une page qui nous permet de rentrer le montant specifique de stock ou la valeur precis que l'on souhaite acheter/vendre puis un bouton pour confirmer la transaction.
- **FR-5 :** Le system doit avoir un onglet qui affiche toutes les anciennes transactions effectuer sur l'application par le user et leur infos. 

---

## 5. Exigences non fonctionnelles (NFR)
> Performance / sécurité / disponibilité / UX / maintenabilité…
- **NFR-1 (Performance) :** <ex. temps de réponse < 2s>
- **NFR-2 (Sécurité) :** <ex. authentification requise>
- **NFR-3 (UX) :** <ex. parcours en ≤ 3 clics>
- **NFR-4 (Qualité) :** <ex. couverture minimale de tests>

---

## 6. Contraintes
- **C-1 (Technologie) :** C# .NET / probablement WPF pour un UI moderne.
- **C-2 (Plateforme) :** Desktop Windows
- **C-3 (Délai) :** Lancement du projet (8 janvier au 25 Janvier 2026), Analyse, exigences et premiers patrons (26 janvier au 22 février 2026), Raffinement architectural et conception avancée (23 février au 22 mars 2026).  Intégration, optimisation et robustesse (23 mars au 19 Avril)
- **C-4 (Outils) :** Git + gestion tâches (Trello/Jira) + Visual Studio

---

## 7. Données & règles métier (si applicable)
- **Entités principales :** <User, Order, ...>
- **Règles métier :** <validation, calculs, permissions, etc.>

---

## 8. Hypothèses & dépendances
### 8.1 Hypothèses
- H-1 : <ex. utilisateurs ont un compte>
- H-2 : <...>

### 8.2 Dépendances
- D-1 : <API externe / BD / service>
- D-2 : <...>

---

## 9. Critères d’acceptation globaux (Definition of Done – mini)
- [ ] Fonctionnalités livrées et testées
- [ ] Tests unitaires présents
- [ ] Gestion d’erreurs minimale
- [ ] Documentation à jour (UML + ADR si requis)
