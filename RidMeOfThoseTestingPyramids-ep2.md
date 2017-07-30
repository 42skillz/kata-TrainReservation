# Virez-moi cette pyramide de tests !
__Thomas PIERRAIN__ (__[use case driven](https://twitter.com/tpierrain)__ on twitter)

> __TL;DR:__ après plus de 12 ans de pratique du TDD, j'ai fini pas adopter presque exclusivement une forme d'*Outside-in* "économe" qui me fait écrire plus de tests d'acceptation que de tests unitaires. Je ne suis donc pas du tout à l'aise avec la pyramide de tests classique que nombreuses personnes revendiquent encore aujourd'hui et qui préconise d'avoir plus de tests unitaires que de tests d'acceptation. Cette série d'articles est une petite visite guidée dans ma tête -et avec du code en soutient - pour vous montrer comment je pratique cette forme d'Outside-In TDD au quotidien. Au détours de l'implémentation de ce kata, cette série sera également l'occasion d'aborder quelques autres sujets autours de ma pratique plus large du développement (DDD, Refactoring,...)

# Episode 2: on continue de faire grandir le système

Quand on s'est quitté __[la dernière fois](./RidMeOfThoseTestingPyramids.md)__, on venait tout juste d'écrire notre second test d'acceptation 

> Should_mark_seats_as_reserved_once_reserved()

Celui-ci avait pour but de vérifier qu'on notifiait bien l'opérateur historique des trains (genre: Hassen Cehef ;-) de notre volonté de réserver des sièges de libre que l'on avait identifié dans un de ses trains. On avait donc pour cela rajouté une nouvelle méthode à l'interface *IProvideTrainData* qui représente l'API de Hassen Cehef et que je rappelle ici :

```C#
public interface IProvideTrainData
{
    List<SeatWithBookingReference> GetSeats(string trainId);
    
    // The new method we want to call at the end of our reservation process
    void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, List<Seat> seats);
}
```

Le test - qui utilise un mock pour ce service de TrainDataProvider pour vérifier qu'on appelle bien cette méthode dans notre cas- était RED, il nous faut donc modifier l'implémentation de notre *TicketOffice* pour pouvoir le faire passer au rouge. 

Trop facile ! On n'a qu'une seule ligne à rajouter en plein milieu de notre méthode *MakeReservation* de la classe *TicketOffice* :

```C#
    this.trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);
```

Uniquement dans le cas où on a effectivement trouvé le bon nombre de place dans le train par contre. Allez, Grand Prince je vous remet ici toute la méthode concernée :

```C#
public class TicketOffice
    {
        // Constructors, fields & Co ...

        public Reservation MakeReservation(ReservationRequest request)
        {
            var reservedSeats = new List<Seat>();
 
            var seats = trainDataProvider.GetSeats(request.TrainId);
            foreach (var seatWithBookingReference in seats)
            {
                if (seatWithBookingReference.IsAvailable())
                {
                    reservedSeats.Add(seatWithBookingReference.Seat);
                }
            }
 
            if (reservedSeats.Count > 0)
            {
                var bookingReference = bookingReferenceProvider.GetBookingReference();

                // LINE ADDED HERE TO FIX THE 2ND ACCEPTANCE TEST
                this.trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);

                return new Reservation(request.TrainId, bookingReference, reservedSeats);
            }
 
            return new Reservation(request.TrainId, string.Empty, reservedSeats);
        }
    }
```

### Et la double-boucle bordel ?!?
Aurais-je du faire un test unitaire ici pour faire passer au vert mon 2nd test d'acceptation ? Je vous laisse méditer dessus un instant mais pour moi la réponse fut assez logiquement non.

Par contre, on ne déconne pas avec le refactoring dans ma famille (celui du TDD notamment: RED-GREEN-__REFACTORING__). Ce fut donc ma préoccupation pour les minutes qui ont suivies. Voyons ça ensemble :

### *Mercyless Refactoring* ? Commence par ranger ta chambre !

Les actions de Refactoring ne sont pas toujours glamour mais elles n'en sont pas moins indispensables. Comme le fait de déplacer les différents types générés à la volée (en mode *As if you mean it* si vous vous souvenez bien du dernier épisode), pour les mettre dans les bons projets et répertoires.

Le feignant efficace que je suis s'est bien entendu servi de son IDE et des outils mis à disposition (ici le plugin R#) pour tout faire en un clin d'oeil. Voici comment je m'y prends en général :

1. Je sélectionne  un projet dans mon explorateur de solution et je __Ctrl-Shift-R__ (*Refactor this!* de R#) pour choisir l'option "*Move types into matching files*". Et hop ! en un instant, toutes les classes et les structs sont réparties dans leur propre fichier au sein du projet en question (pratique pour n'en oublier aucune). J'appelle ça "*déplier*" mon projet.
2. Il ne me reste ensuite plus qu'à leur faire des moves. Là encore une fois, le partisan du moindre effort -pour les tâches stupides- que je suis ne le fait pas fichier par fichier mais sélectionne d'un seul coup dans l'explorateur de solution tous ceux que j'ai envie de bouger dans le même répertoire de destination -> __Ctrl-Shift-R__ à nouveau, pour choisir cette fois le "*Move to Folder*" (F6 pour les intimes) et laisser mon "*Richard*" préféré faire tous les changements de namespaces qui vont bien (côté déclaration du type mais aussi surtout du côté de tous les endroits qui l'utilisent).

### Certains l'aiment *snob*
Dans certaines conférences, j'ai régulièrement entendu un discours anti-IDE / anti-outil de refactoring pour promouvoir à la place l'usage de la *b... et du couteau*. Quand on voit le temps gagné par exemple sur l'opération de Move décrite précédemment, je dois avouer que je suis perplexe face à cette tendance qui se rapproche d'une forme de frime (ou de snobisme dans certains cas). Ne dit-on pas que *les bons outils font les bons artisans* ? (oui je sais, on dit aussi que *Pierre qui roule n'amasse pas mousse* et que *un vaut mieux que 2 tu l'auras*, mais quand même...)

### Le test, cet incompris
Quand on parle de Refactoring, on parle assez peu souvent du refactoring de nos tests, et c'est bien dommage. Parce que des tests fastidieux à lire sont usants à la longue. Ca fatigue tout le monde dans l'équipe, et ça donne des excuses aux professionnels du *ralage* pour vous intoxiquer vos journées (je suis persuadé que vous en avez croisé vous aussi dans vos projets ;-). C'est pour éviter cette situation que j'ai remplacé les 2 lignes suivantes dans mon 1er test d'acceptation :

```C#

var trainDataProvider = Substitute.For<IProvideTrainData>();
trainDataProvider.GetSeats(trainId).Returns(new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) });

```

par une version beaucoup plus expressive et moins fatigante à lire à la longue :

```C#

var trainDataProvider = Substitute.For<IProvideTrainData>();
trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1CoachAnd3SeatsAvailable());

```

avec la petite méthode helper qui va bien juste en dessous (méthode statique de la classe de Tests d'ici à ce qu'on ai besoin de la réutiliser/déplacer par la suite dans un helper du genre: *TrainTopologyGenerator*) :

```C#

private static List<SeatWithBookingReference> GetTrainWith1CoachAnd3SeatsAvailable()
{
    return new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) };
}

```

J'ai fait la même chose pour le second test d'acceptation mais je ne vous mets ici que le nom de la méthode helper correspondante, parce que sinon ça va commencer à devenir un peu chiant. Il s'agit de : 

> GetTrainWith1Coach3SeatsIncluding2Available()

Ok. Si on fait un rapide *point route* à ce niveau de notre avancement, on peut dire qu'on a fait un minimum de rangement dans nos projets, puis un tout petit peu de  refatoring (mais dans nos tests d'acceptation cette fois). Il est donc temps de passer à la suite, à savoir : l'écriture de notre 3eme test d'acceptation.

## 3eme test d'acceptation

Pour rappel :
- notre 1er test d'acceptation a montré qu'il était possible d'identifier des sièges adaptés à notre demande de réservation dans le cas trivial d'un train vide.

- notre second test d'acceptation a montré qu'une fois les places identifiées pour une demande de reservation, notre système sollicitait ensuite comme prévu le back-end TrainDataService de l'opérateur historique (manipulé comme un IProvideTrainData par notre logique métier) pour "commiter" la transaction.

### Nos tests, cette dynamique de Design
Jusqu'à maintenant, j'ai surtout utilisé mes tests d'acceptation pour dessiner les contours de mon système considéré depuis l'extérieur comme une grosse boite noire. Cela m'a permis de définir progressivement comment on interagit avec elle (1er test) et comment elle interagit avec les services extérieurs (comme ici dans le second test avec les services de  *Hassen Cehef*). __A ce moment de mon avancement, les contours de mon système sont encore grossiers, mais suffisants pour me rassurer__. C'est pour cette raison que j'ai décidé à cet instant-là de commencer à "muscler" la logique métier à l'intérieur de ma boîte, en y intégrant cette fois le support de l'invariant métier : "*on ne doit pas remplir un train à plus de 70% de sa capacité totale*". 

### Le moment "*vieux con*"
*Quand j'étais plus jeune...* il m'arrivait de partir bille en tête dans l'écriture d'un test malgré le fait que son nom (et donc son comportement) ne soit pas clair. Dans ces moments-là je tâtonnais plus ou moins longtemps, jusqu'à ce que j'identifie clairement son objectif (par contre dites pas à ma mère que je faisais comme ça svp). Avec les années j'ai compris que ce n'étais pas optimal, et ai décidé de me munir plus souvent d'un petit bout de papier et d'un crayon à côté de moi quand je code, pour pouvoir clarifier certains points et trouver des exemples concrets (j'ai très souvent besoin de dessiner quelque chose pour pouvoir l'assimiler, le digérer). Ici, cela m'a servi à trouver l'exemple le plus simple possible pour vérifier cet invariant métier : un train avec une seule voiture de 10 places, dans lequel je ne peux réserver que 7 places. 

![7 places pour la 12](./images/train7places.jpg)

### Eviter l'effet *magic number*
Autre point important ici : quand on fait du TDD, il est primordial de choisir les cas d'exemples les plus simples possibles pour qu'un nouvel arrivant sur le projet ne s'imagine pas des trucs en lisant nos tests. De s'imaginer par exemple qu'il y a un cas particulier implicite au-delà de ce qu'on teste. En d'autres termes, on veut éviter des situations où le lecteur se dit : "*pourquoi ils ont utilisé le chiffre 3492 ici, est-ce que ce dernier correspond à un cas important pour le métier ?!?*" (alors que en fait non, c'était juste pour déconner ! ;-)

C'est pour cette raison que j'ai choisi ici le cas qui m'apparaissait comme le plus simple pour vérifier la règle des 70% (1 voiture, 10 places de libre).

Voici donc le test d'acceptation correspondant :

```C#

[Test]
public void Should_not_reserve_more_than_70_percent_of_seats_for_overall_train()
{
    // setup mocks
    var expectedBookingId = "75bcd15";
    var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
    bookingReferenceProvider.GetBookingReference().Returns(new BookingReference(expectedBookingId));

    var trainId = "express_2000";
    var trainDataProvider = Substitute.For<IProvideTrainData>();
    trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1CoachAnd10SeatsAvailable());

    // act
    var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
    var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, seatCount: 10));

    Check.That(reservation.TrainId).IsEqualTo(trainId);
    Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
    Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3), new Seat("A", 4), new Seat("A", 5), new Seat("A", 6), new Seat("A", 7));
}

```

Au passage, j'en profite pour redéfinir le ToString() de la classe *Seat*, pour que mes assertions soient plus lisibles en cas d'erreur. Un truc simple du genre:

```C#
    // Seat class ...
    public override string ToString()
    {
        return $"{this.Coach}{this.SeatNumber}";
    }
```

Bon. A ce moment du récit, quelque chose devrait vous gêner  normalement. 

En effet, si on y regarde à 2 fois, le précédent test est un peu stupide. Pourquoi ? allez, je vous laisse 30 secondes pour trouver tout seul avant de lire la suite.

![jeopardy](https://media.giphy.com/media/xHgVaJ9zySg80/giphy.gif)

### Le respect du client
Vous voyez le problème maintenant ? C'est en effet le contrat de mon API qui est pourri. Ici, le père de famille nombreuse que je suis demande à réserver 10 places et se voit répondre par le système de réservation: "*on t'en a trouvé et réservé 7*"... Génial, et je fais quoi alors de mes autres enfants maintenant... En terme d'expérience utilisateur, je crois qu'il n'y a guère que le service de la Poste de mon quartier qui fait mieux (avec leurs avis de passages systématiquement mis dans notre boite à lettre alors qu'on a pris la journée pour les attendre à la maison ;(

### Pair programming
Avec le recul, je suis persuadé que je n'aurais pas pris ce premier exemple un peu couillon si j'avais travaillé *en binôme* avec quelqu'un. Mais bon là, il était tard le soir, pas trop le choix.

### Le TDD est ton ami
Mais ce n'est pas grave, car la force du TDD est de nous permettre d'avancer sereinement et d'améliorer notre système pas à pas, même en cas de moments d'absence comme ici. Avec le TDD, tous nos pas sont également tracés. On peut donc se libérer très facilement d'une grosse partie de la charge cognitive qu'on a parfois quand on travaille trop longtemps sur un truc qui tient uniquement dans notre tête (*REPL* si tu m'entends...), histoire de dédié sa propre CPU sur un problème local restreint, et donc gérable (le fameux "*diviser pour mieux régner*").

Bon. Ce qui est fait est fait, et comme je me sers d'une expérience passée pour rédiger cet article ([voir les détails ici dans le 1er épisode](.RidMeOfThoseTestingPyramids.md)), continuons d'avancer. Et puis on va pouvoir rectifier cette bêtise assez vite par la suite.

### Se mettre au vert, encore une fois

RED-__GREEN__-REFACTOR. Pour passer au vert ce nouveau test d'acceptation, j'ai introduit ici le concept de *Train* (qui fait partie du Domaine) et le concept de *ReservationOption*. 

### Les raisins de la colère
Le *Train* dans ce contexte va représenter ce que le DDD nomme un *Agrégat*, c'est à dire un ensemble (une grappe) d'objets qui va former un tout cohérent du point de vue métier. Rester cohérent, c'est avant tout être capable de changer, d'évoluer ensemble en tant que grappe mais de manière consistante, en respectant toujours un ou plusieurs *invariants métiers* (comme la règle des 70%). 

#### Sans invariant métier, on ne parle pas d'*aggrégat*. 

Quand on manipule un *Agrégat* on ne doit parler qu'à son objet racine (*AggregateRoot*) qui est sa façade et le garant de cette consistance métier. Celui-ci n'exposera donc pas les détails de son implémentation. Il sera donc le seul objet "accessible en modification" vis à vis du reste du programme.

Ici l'*AggregateRoot* *Train* va être en charge de faire respecter l'invariant métier : "*On ne doit pas réserver plus de 70% de la capacité totale d'un train*". Pour ce faire, il va agréger et manipuler en interne plusieurs instances de *Seat* qui ne seront elles pas accessibles directement par mon programme pour éviter qu'un autre bout de code ne vienne lui faire un enfant dans le dos et violer gaiement tous les invariants métiers qu'il s'escrime à garantir. Pour être plus concret, imaginez ici un autre bout de code qui irait en louzedé directement changer la Booking reference ou le IsAvailable d'un des sièges de l'Aggrégat *Train*, et violer ainsi la règle des 70%. Sad Panda. C'est pour éviter ça que l'objet racine de l'agrégat encapsule comme il se doit le reste de l'agrégat.

### "Expliciter les implicites"
L'autre concept de *ReservationOption* que j'ai rajouté à ce stade permet d'expliciter maintenant clairement que ce que mon système cherche avant tout à identifier au sein des *Trains*, c'est une "*possibilité de réservation*", une "*réservation en devenir*". En effet, tant que celle-ci n'a pas été confirmée auprès du back-end extérieur *TrainDataService* de *Hassen Cehef*, ce n'est qu'une hypothèse faite par mon système. "*Expliciter les implicites*" est un des mantras du DDD pour éviter les incompréhensions, les erreurs mais surtout pour coller au plus près au besoin du métier pour lequel on travaille (être pertinent en définitive).

Pour reprendre le cas de la *ReservationOption*, celle-ci va remplacer nos instances de *List<Seat>* dans la classe principale *TicketOffice*. Son introduction va également nous permettre de remplacer le :

````C#
if (reservedSeats.Count == requested.SeatCount)
````
par un :

````C#
if (reservationOption.IsFullfiled)
````

Beaucoup plus orienté *métier* en tout cas.

### Un code lisible pour tous (y compris le métier)

Avec mon ami et associé [Bruno BOUCARD](https://twitter.com/brunoboucard), on a l'habitude de répéter que *le binaire c'est pour les machines, le code c'est pour les gens*. Quand je code la partie métier, j'essaie le plus possible de faire en sorte qu'un expert du domaine puisse lire mon code et le comprendre. Même si tous les experts du domaine n'auront pas envie de lire du code dans un IDE, cela me permet au moins de leur lire celui-ci sans jargon technique et sans me fatiguer ou perdre en traduction technique/métier à chaque fois que je veux leur décrire ce que le système réalise ou va réaliser. Le code du domaine doit pour moi ressembler un peu à une histoire qu'on peut lire de manière fluide. 

#### 1 ligne de code = 1 intention
Je crois que j'ai été traumatisé dans mes expériences passées par des codebases (C, C++ mais aussi java ou C#) ou le retour à la ligne systématique pour chaque argument de chaque appel de fonction/méthode menait à des situations où la hauteur d'un écran était complètement prise par l'appel d'une ou de deux méthodes seulement. Cela date d'une période où nos résolutions d'écrans étaient ridiculement petites par rapport à ce qu'on a aujourd'hui, mais je trouve encore régulièrement des collègues qui continuent à appliquer ces conventions. 

 De mon point de vue, celles-ci rendent plus compliquée la lecture et l'appréciation de la taille d'une codebase sans outil. En effet dans ce cas-là, la hauteur d'un écran peut soit recouvrer pleins d'intentions/d'étapes, soit juste 2 ou 3 (quand on utilise intensivement ces line-wraps). Pour mon cerveau qui fait en permanence du pattern matching visuel, cette asymétrie est pénible. C'est donc pour cela que j'aime bien avoir 1 ligne de code = 1 intention. 

Mais je m'égare un peu; ici nous parlions du fait de rendre notre code du domaine lisible par des non développeurs. Le :
```C#
if (reservationOption.IsFullfiled)
``` 
est à mon avis beaucoup plus explicite que l'ancien ==. Cette dernière version emprunte également directement la terminologie du métier dans le code (le concept d'*Ubiquitous Language* en DDD).
Au passage, dernière petite parenthèse pour ceux qui n'auraient jamais fait de C#, le *IsFullfiled* ici est ce qu'on appelle une *Property* en .NET, c'est en fait une méthode mais dont l'expressivité (sans parenthèse) permet d'améliorer la lisibilité dans le code. On s'en sert en général quand on veut encapsuler une variable membre privée (que l'on appelle *field* en .NET) mais sans pour autant avoir un code boiler-plate avec des Get et des Set dans le nom des méthodes associées. Ok, fin de la parenthèse C# ;-)

### Dynamique de conception
Revenons à nos moutons. Oui, reconcentrons-nous sur la dynamique du TDD. Ici, tout est parti à l'intérieur de la méthode :

> public Reservation MakeReservation(ReservationRequest request)

 du type  *TicketOffice* à partir duquel j'ai construit à la volée ce dont j'avais besoin pour passer ce 3eme test d'acceptation au vert ("make it pass"). 
 
 Au passage, vous noterez que la méthode retourne bien toujours une *Reservation* et non pas la *ReservationOption* dont on vient de parler. C'est normal, car le résultat de cette méthode est bel et bien une *Reservation* officielle. C'est lié au fait que notre processus de réservation inclut la confirmation de celle-ci auprès des back-ends de l'opérateur historique qui gère les trains (*Hassen Cechef*). En gros, on va travailler ici à identifier une *ReservationOption* à partir d'une topologie de *Train*, et cette option sera ensuite transformée en véritable *Reservation* à la fin de la méthode (ce que demande nos passagers).

#### 1ere étape : implémentation de nouveaux concepts
Le train data provider qui me retournait une liste de Sièges (*List<Seat>*) va donc me retourner désormais une instance de type *Train* qui n'existe pas encore. La méthode *MakeReservation* commence donc maintenant par :
```C#

var train = trainDataProvider.GetTrain(request.TrainId);

// au lieu du précédent:
var reservedSeats = new List<Seat>();

```
La construction du type *Train* part donc de cette nouvelle ligne de code qui ne compilera pas pendant à peine une seconde le temps que je finisse de l'écrire, et que j'utilise dans la foulée un de mes raccourcis préférés de R#: *Alt-Enter* (qui montre les actions possibles en fonction du contexte. Ici, '*create type Train*').

#### Comme une plante
A ce moment-là, je ne remplace pas encore l'ancien code de la méthode *TicketOffice.MakeReservation* par le nouveau. Je ne fais que rajouter le nouveau juste au dessus de l'ancien dans la méthode *MakeReservation*. Cela me permet du coup de "faire pousser" la nouvelle implémentation à côté de l'ancienne qui sera toujours d'usage jusqu'à ce que la nouvelle soit opérante. Ceci est indispensable pour éviter de péter mes précédents tests d'acceptance pendant la manip.

Une fois le type *Train* créé, je reviens à la ligne qui m'a servi de point de départ pour le récupérer (à l'aide du raccourci magique Ctrl-Shift-Backspace) et je vais demander ensuite à cette nouvelle instance de Train de me retourner une option de réservation à la ligne suivante. Je créé donc la méthode *Train.Reserve* à la volée au moment où j'écris la ligne :

```C#
var option = train.Reserve(request.SeatCount);
```

(note: on se trouve toujours au début de la méthode *MakeReservation* du *TicketOffice*)

Une fois cette méthode *Reserve* crée, je peux très facilement l'implémenter en copiant-collant la portion de code legacy ci-dessous qui je vous le rappelle est toujours l'implémentation officielle (enfin celle qui retourne le résultat final pour l'instant) :

```C#
    var seats = trainDataProvider.GetSeats(request.TrainId);
    foreach (var seatWithBookingReference in seats)
    {
        if (seatWithBookingReference.IsAvailable())
        {
            reservedSeats.Add(seatWithBookingReference.Seat);
        }
    }
```

La seule chose à adapter pour finaliser l'implémentation de la méthode *Train.Reserve(..)* sera de ne pas retourner une liste de sièges, mais bel et bien une instance d'un nouveau type *ReservationOption* que je vais façonner dans les secondes qui suivent (toujours en utilisant activement le raccourci *Alt-Enter* pour y créer constructeurs, champs privés et Propriétés).

A ce moment là, voici à quoi ressemblent mes classes *Train* et *ReservationOption*:

Train.cs :

```C#
namespace TrainReservation.Domain
{
    public class Train
    {
        private readonly List<SeatWithBookingReference> seatWithBookingReferences;
        public string TrainId { get; }
        
        // Constructor
        public Train(string trainId, List<SeatWithBookingReference> seatWithBookingReferences)
        {
            this.seatWithBookingReferences = seatWithBookingReferences;
            TrainId = trainId;
        }

        public ReservationOption Reserve(int requestSeatCount)
        {
            var option = new ReservationOption(this.TrainId, requestSeatCount);
            foreach (var seat in this.seatWithBookingReferences)
            {
                if (seat.IsAvailable())
                {
                    option.AddSeatReservation(seat.Seat);
                    if (option.Fullfiled)
                    {
                        break;
                    }
                }
            }

            return option;
        }
    }
}
```

ReservationOption.cs : 
```C#
namespace TrainReservation.Domain
{
    public class ReservationOption
    {
        private readonly string trainId;
        private readonly int requestSeatCount;
        private readonly Seats reservedSeats;

        public bool Fullfiled { get; private set; }
        public Seats ReservedSeats { get { return this.reservedSeats; } }

        public ReservationOption(string trainId, int requestSeatCount)
        {
            this.trainId = trainId;
            this.requestSeatCount = requestSeatCount;
            this.reservedSeats = new Seats();
        }

        public void AddSeatReservation(Seat seat)
        {
            if (!this.Fullfiled)
            {
                this.reservedSeats.Add(seat);
                if (this.reservedSeats.Count == this.requestSeatCount)
                {
                    this.Fullfiled = true;
                }
            }
        }
    }
}
```

Ceux d'entre-vous qui sont les plus observateurs auront peut-être noté l'introduction d'un nouveau type au passage : *Seats* (et non pas le *Seat* existant). Ceci était juste un moyen pour moi d'exposer __explicitement__ le résultat d'une Option de réservation en mode lecture seule (sans exposer de *List<Seat>*).

Voici donc pour le nouveau code qui pousse tout seul à côté de l'implémentation officielle. Maintenant si on regarde à quoi ressemble ma méthode *MakeReservation* du *TicketOffice* à ce moment-là de l'action, on a ça :

```C#
    public Reservation MakeReservation(ReservationRequest request)
    {
        // New implementation that I'm currently making grow
        var train = trainDataProvider.GetTrain(request.TrainId);
        var option = train.Reserve(request.SeatCount);

        // ----------------------------------------------------------
        // The former implementation (still in charge here)
        var reservedSeats = new List<Seat>();

        var seats = trainDataProvider.GetSeats(request.TrainId);
        foreach (var seatWithBookingReference in seats)
        {
            if (seatWithBookingReference.IsAvailable())
            {
                reservedSeats.Add(seatWithBookingReference.Seat);
            }
        }

        if (reservedSeats.Count > 0)
        {
            var bookingReference = bookingReferenceProvider.GetBookingReference();

            trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);

            return new Reservation(request.TrainId, bookingReference, reservedSeats);
        }

        return new Reservation(request.TrainId, string.Empty, reservedSeats);
    }

```

En un rien de temps, le nouveau code qui manipule un *Train* et une option de réservation (*ReservationOption*) est presque prêt à prendre la relève de l'ancien au sein de la méthode *TicketOffice.MakeReservation*.

#### Mais he... la double boucle... ?!?

Une question légitime ici est : aurais-je du écrire en cours de route des tests unitaires pour mes nouveaux type *Train* et *ReservationOption* ? 

Oui j'aurai pu, mais je n'en ai pas ressenti ici le besoin tant mes idées sur l'implémentation étaient claires et tant le nouveau code était presque un léger refactoring de l'ancien (vous vous rappelez, j'ai recyclé intégralement la boucle qui recherche les sièges de libre à travers tous les sièges d'un train). A l'aide de R#, tout s'est effectué très vite, en moins de 10 minutes - le tout sans faire péter mes tests d'acceptation existants.

Par contre, j'introduirai certains tests unitaires sur le *Train* par la suite. Vous le verrez donc, cette décision d'écrire -ou pas- un test unitaire sous mon test d'acceptation n'est pas gravée dans le marbre, elle est contextuelle (quand je n'ai pas les idées claires ou que je tombe sur une difficulté pendant plus de 5 minutes).

#### 2eme étape : on remplace l'ancien code par le nouveau
Oui, après une dizaine de minutes à coder ces 2 nouveaux concepts à côté du code legacy de notre méthode *Train.MakeReservation*, il est temps de voir si ce qu'on a écrits peut remplacer l'ancien code. Pour l'instant, j'ai préparé le terrain pour implémenter la règle de gestion au niveau du train, mais comme j'ai besoin de faire des baby steps pour avancer sereinement (et éviter un effet tunnel), je préfère m'assurer que ces nouveaux concepts qui vont me servir de base pour la suite s'insèrent bien dans mon code. 

Voici donc en cet instant à quoi ressemble la méthode *MakeReservation* de la classe *TicketOffice* :

```C#
    public Reservation MakeReservation(ReservationRequest request)
    {
        var train = trainDataProvider.GetTrain(request.TrainId);
        var option = train.Reserve(request.SeatCount);

        if (option.IsFullfiled)
        {
            var bookingReference = bookingReferenceProvider.GetBookingReference();

            trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, option.ReservedSeats);
            return new Reservation(request.TrainId, bookingReference, option.ReservedSeats);
        }
        else
        {
            return new Reservation(request.TrainId);
        }
    }
```

Plus clair, non ? Il va sans dire que si mon 3eme test d'acceptation est toujours rouge (je n'ai pas encore attaqué l'implémentation de la règle des 70%), les 2 premiers sont toujours verts.

### Mais c'est qu'il va même nous péter le RED-GREEN-REFACTOR maintenant ?!?
Vous l'aurez remarqué, j'ai introduit ici une étape intermédiaire avant de fixer mon test d'acceptance. J'ai fait ce que mon ami __[Philippe BOURGAU](https://twitter.com/pbourgau)__ explique très bien dans son récent post __[Don't Stick to TDD's Red-Green-Refactor Loop to the Letter](http://philippe.bourgau.net/dont-stick-to-tdds-red-green-refactor-loop-to-the-letter/
)__ : un RED-REFACTOR-RED-GREEN ;-)

#### Un moment de faiblesse (mais tout de même sous contrôle)

D'habitude quand je fais ça, je commente le nouveau test (celui qui est rouge) pour pouvoir m'assurer que je ne pète pas les anciens pendant ma phase de refactoring (que je reste GREEN donc). Ici, j'ai fait une chose équivalente en monitorant visuellement l'outil qui m'exécute en permanence mes tests tout au long de mes modifications du code : le très addictif __[NCrunch](http://www.ncrunch.net/)__ . J'étais bien ROUGE d'après lui, mais celui-ci m'indiquait dans la marge de mon IDE que je n'avais toujours qu'un seul test qui échouait lors de mon introduction de ces nouveaux concepts à la marge (*Train* et *ReservationOptions*). Pas très orthodoxe je vous l'accorde, et à mon avis lié à l'heure tardive de mon intervention. Si c'était à refaire, je ferai plutôt comme d'habitude en ignorant le 3eme test d'acceptance pour rester GREEN tout au long de mon REFACTORING intermédiaire, avant de le réactiver -et donc de redevenir RED- au moment d'introduire véritablement la règle de gestion des 70%.

Au passage, si vous n'avez pas encore découvert le blog de Philippe, je vous encourage vivement à le faire tant ce dernier regorge de nombreuses pépites dans la veine de l'ouvrage (indémodable) de référence : __[The Pragmatic Programmer](https://en.wikipedia.org/wiki/The_Pragmatic_Programmer)__.

### Mais il va l'écrire son 1er test unitaire, ou bien ?!?
C'est vrai qu'à ce stade d'avancement, certains d'entre-vous se diront sans doute : va-t-il continuer à ne pas faire de test unitaire et à se reposer uniquement sur ses tests d'acceptation comme depuis le début ? 

La question que j'aurai envie de vous objecter est : pensez-vous qu'il est nécessaire à ce stade d'introduire un ou plusieurs tests unitaires sur le *Train* pour vérifier la même chose (mais au niveau plus bas, c.ad. sans l'utilisation préalable du *IProvideTrainData.GetTrain(string trainId)*) que ce qui est déjà illustré dans mon 3eme test d'acceptation ?

Je ne suis pas en train de dire que je ne vais pas en écrire, je suis juste en train de dire que ce n'était pas le bon moment pour moi : __je n'étais pas prêt à payer le prix d'une telle duplication alors même que mes idées sur le design du système n'étaient pas encore très matures ni arrêtées__. Je préférais pour l'heure concentrer mes efforts sur le comportement et les contours du système dans son ensemble, beaucoup plus rentable de mon point de vue.

### Bon, on se l'implémente cette règle des 70% ?!?
Oui, maintenant que le concept de *Train* (agrégat) existe et a remplacé le vieux code qui ne parlais que de *List<Seat>*, on va pouvoir enfin le faire implémenter la règle qui veut qu'on ne doit pas remplir un train à plus de 70% de sa capacité. Cool ! Sauf que...

#### Er... I've made a huge mistake
Oui, c'est à ce moment-là que je me suis rendu compte que mon 3eme test d'acceptation était vraiment stupide (vous vous rappelez, c'est celui qui préconisait qu'on devait quand même réserver 7 places alors que je lui en demandais 10 pour ma famille nombreuse).

J'ai donc :

- Renommé et changé ce 3eme test d'acceptation pour qu'il ne soit plus stupide et qu'il vérifie juste qu'on est bien en mesure de réserver 7 places sur un train de 10 places vides.
    - Déjà GREEN avant que je n'implémente la règle des 70%. Je l'ai laissé ici pour m'assurer que mon implémentation à venir ne cassera pas cet état de fait.
- Rajouté un nouveau test d'acceptation plus pertinant qui illustre le fait que le système ne permette pas de réserver plus de 70% des places d'un train vide (de 10 places).

Voici à quoi ressemble ce nouveau test d'acceptation :

```C#
[Test]
public void Should_fail_to_reserve_when_asking_more_seats_than_the_70_percent_limit()
{
    // setup mocks
    var expectedBookingId = "75bcd15";
    var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(expectedBookingId);

    var trainId = "express_2000";
    var trainDataProvider = Substitute.For<IProvideTrainData>();
    trainDataProvider.GetTrainSnapshot(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

    // act
    var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
    var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, seatCount: 8));

    // note: reservation has failed here
    Check.That(reservation.TrainId).IsEqualTo(trainId);
    Check.That(reservation.BookingReference.Value).IsEmpty();
    Check.That(reservation.Seats).IsEmpty();
}

```

Et c'est bien le seul test qui échoue à ce moment-là. Tout est donc vraiment en place pour que j'implémente l'invariant métier des 70% dans mon agrégat *Train*. 

J'ai donc commencé cette implémentation en écrivant les 2 vérifications (__[guard clause](http://wiki.c2.com/?GuardClause)__) suivantes au début de la méthode *Train.Reserve* :

```C#
if (AvailableSeatsCount < requestedSeatCount)
{
    return option;
}

if (AvailableSeatsCount - requestedSeatCount > MaxReservableSeatsFollowingThePolicy)
{
    return option;
}

```

C'est en écrivant ces 2 conditions portant sur les propriétés *AvailableSeatsCount* et *MaxReservableSeatsFollowingThePolicy*  jusqu’alors inexistantes sur le *Train* que j'ai ressenti le besoin d'écrire mon 1er test unitaire (sur l'agrégat *Train* donc).

A ce moment là de l'histoire, j'ai juste fait compiler l'ensemble en utilisant R# (*Alt-Enter -> create property "AvailableSeatsCount"* et *Alt-Enter -> create property "MaxReservableSeatsFollowingThePolicy"*), et laissé à chaque fois les *NotImplementedException* par défaut.


### Ah... enfin un test unitaire !

Et oui, voici donc cette fameuse petite boucle qu'on évoque depuis le début et que j'ai attendu si longtemps à mettre en oeuvre. Ici, ce n'est pas la complexité de l'implémentation de ces propriétés du *Train* (assez triviales) qui m'ont fait écrire ce petit test unitaire, mais plutôt :
 
 - __la pérennité du type *Train*:__ le fait que ce sujet à tester soit à la fois un concept du domaine et un agrégat est un gage de pérennité. Tester un agrégat est en général plus pérenne que de tester une toute petite classe dont on ne sait pas si elle va survivre ou pas à notre travail quotidien sur l'implémentation.
 - __mon besoin de baby steps à ce moment-là__ : je pense que mon état de fatigue du moment a dû jouer sur ma volonté de décomposer un peu plus mes actions en baby-steps. 

Voici donc à quoi ressemblait ce 1er test unitaire : 

```C#
using NFluent;
using NUnit.Framework;
using TrainReservation.Tests.Helpers;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class TrainTests
    {
        [Test]
        public void Should_return_70_percent_of_OverallTrainCapacity_as_MaxReservableSeatsFollowingThePolicy()
        {
            var train = TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable("trainId42");
            Check.That(train.OverallTrainCapacity).IsEqualTo(10);
            Check.That(train.MaxReservableSeatsFollowingThePolicy).IsEqualTo(7);
        }
    }
}
```

qui m'a permis d'implémenter à la fois l'*OverallTrainCapacity* d'un *Train*, mais aussi sa propriété *MaxReservableSeatsFollowingThePolicy*. Voici leur code à l'époque :

```C#
// new private fields
private const double SeventyPercent = 0.70d;
private readonly List<SeatWithBookingReference> seatsWithBookingReferences;
        
// New properties
public int OverallTrainCapacity  => seatsWithBookingReferences.Count; 

public int MaxReservableSeatsFollowingThePolicy => (int)Math.Round(OverallTrainCapacity * SeventyPercent);

```

Note : en C#, on peut remplacer les propriétés de type *get* par la notation *expression-bodied Property* avec un formalisme utilisant une lambda (=>). C'est très exactement ce que j'ai utilisé ici pour les 2 propriétés.

Mettre au vert ce test unitaire fut un premier pas pour que mon dernier test d'acceptation passe au vert lui aussi, mais pas suffisant. Pour cela, il m'a fallu remplacer le *NotImplementedException* de la propriété *AvailableSeatsCount* par le code suivant :

```C#

public int AvailableSeatsCount
{
    get
    {
        var availableSeatsCount = 0;
        foreach (var seatWithBookingReference in seatsWithBookingReferences)
        {
            if (seatWithBookingReference.IsAvailable)
            {
                availableSeatsCount++;
            }
        }

        return availableSeatsCount;
    }
}

```

C'est suite à cet ajout que mon dernier test d'acceptance (*Should_fail_to_reserve_when_asking_more_seats_than_the_70_percent_limit()*) passa au vert.

On l'a dit plus tôt, on ne badine pas avec le refactoring dans ma famille, alors ici j'ai profité que tous mes tests soient verts pour remplacer le code procédural que je venais de rajouter pour la propriété *AvailableSeatsCount* par du code plus fonctionnel à l'aide d'opérateurs LINQ (l'équivalent des streams pour ceux qui feraient du java). Voici donc la nouvelle forme de cette propriété : 

```C#
public int AvailableSeatsCount => seatsWithBookingReferences.Count(seatWithBookingReference => seatWithBookingReference.IsAvailable);

``` 

### Auto-critique
Même si elle était déjà couverte au niveau global par mes tests d'acceptation, j'avoue que j'aurai pu ici tester cette propriété *AvailableSeatsCount* en mode unitaire avant d'écrire son implémentation dans la classe *Train*. Si c'était à refaire, c'est ce que je ferai.

Le fait d'avoir désormais une classe de test dédiée au *Train* me permettra à l'avenir d'améliorer celui-ci en mode unitaire pour continuer à le faire grandir. Mais tout ça, je vous propose de le voir ensemble dans un prochain épisode.

D'ici là, je vous livre la version complète de mon agrégat *Train* au moment où mes 4 premiers tests d'acceptation étaient GREEN :

```C#
namespace TrainReservation.Domain
{
    public class Train
    {
        private const double SeventyPercent = 0.70d;
        private readonly List<SeatWithBookingReference> seatsWithBookingReferences;
        public string TrainId { get; }

        public Train(string trainId, List<SeatWithBookingReference> seatsWithBookingReferences)
        {
            this.seatsWithBookingReferences = seatsWithBookingReferences;
            TrainId = trainId;
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(TrainId, requestedSeatCount);

            if (AvailableSeatsCount < requestedSeatCount)
            {
                return option;
            }

            if (AvailableSeatsCount - requestedSeatCount > MaxReservableSeatsFollowingThePolicy)
            {
                return option;
            }

            foreach (var seat in seatsWithBookingReferences)
            {
                if (seat.IsAvailable)
                {
                    option.AddSeatReservation(seat.Seat);
                    if (option.IsFullfiled)
                    {
                        break;
                    }
                }
            }

            return option;
        }

        public int OverallTrainCapacity { get { return seatsWithBookingReferences.Count; } }

        public int MaxReservableSeatsFollowingThePolicy => (int)Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AvailableSeatsCount => seatsWithBookingReferences.Count(seatWithBookingReference => seatWithBookingReference.IsAvailable);
    }
}
```



---

