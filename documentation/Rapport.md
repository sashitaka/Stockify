
# Rapport Stockify

### Diagrammes

Diagramme de classe

Classe user qui represente un utilisateur avec comme attributs ses informations de compte et une liste de stock du type PortfolioItem. PortfolioItem represente une stock que le user possede, cette classe herite de la classe stock. La classe stock represente une stock typique. Classe transactions represente une transaction effectuer par le user vente ou achat d'une stock contenant les infos pertinentes de la transac. La classe admin sera un user qui pour gerer l'application notamment supprimer un user client, d'autres actions seront defini au fur et a mesure. Finalement la class Api manager qui est utilise le patron de conception Singleton qui implemente la classe interface stockService pour utiliser ses methodes.

![Diagramme de classe Image](ImgDiagrammeClass.png)



Diagramme de Cas d'utilisation
Ce diagramme montre les principales actions possibles dans l’application Stockify. Il y a deux acteurs : l’utilisateur et l’administrateur. Les deux peuvent se connecter, naviguer dans la liste des stocks et se déconnecter. L’utilisateur, lui, peut en plus acheter des stocks, consulter son portefeuille, vendre des actions et voir l’historique de ses transactions.

![Diagramme Cas d'utilisation](CasDutilisation.png)


Diagramme de composantes

![Diagramme Composantes](diagramme_Composant.png)



Diagramme d'entite relationnelle

![Diagramme Entite relationnelle](ImgDiagrammeEntiteRelation.PNG)
