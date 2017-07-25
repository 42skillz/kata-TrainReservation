# Virez-moi cette pyramide de tests !
__Thomas PIERRAIN__ (__[use case driven](https://twitter.com/tpierrain)__ on twitter)

> __TL;DR:__ après plus de 12 ans de pratique du TDD, j'ai fini pas adopter presque exclusivement une forme d'*Outside-in* "économe" qui me fait écrire plus de tests d'acceptation que de tests unitaires. Je ne suis donc pas du tout à l'aise avec la pyramide de tests classique que nombreuses personnes revendiquent encore aujourd'hui et qui préconise d'avoir plus de tests unitaires que de tests d'acceptation. Cette série d'articles est une petite visite guidée dans ma tête -et avec du code en soutient - pour vous montrer comment je pratique cette forme d'Outside-In TDD au quotidien.

# Episode 2: on continue de faire grandir le système

Quand on s'est quitté __[la dernière fois](./RidMeOfThoseTestingPyramids.md)__, on venait tout juste d'écrire notre second test d'acceptation 

> Should_mark_seats_as_reserved_once_reserved()

Celui-ci avait pour but de vérifier qu'on notifiait bien l'opérateur historique des trains (genre: Hassen Cehef ;-) de notre volonté de réserver des sièges de libre que l'on avait identifié dans un de ses trains. On avait donc pour celà rajouté une nouvelle méthode à l'interface *IProvideTrainData* que je rappelle ici:

```C#
public interface IProvideTrainData
{
    List<SeatWithBookingReference> GetSeats(string trainId);
    
    // The new method we want to call at the end of our reservation process
    void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, List<Seat> seats);
}
```

Le test - qui utilise un mock pour ce service de TrainDataProvider pour vérifier qu'on appelle bien cette méthode dans notre cas- était RED, il nous faut donc modifier l'implémentation de notre *TicketOffice* pour pouvoir le faire passer au rouge. 

Trop facile ! On n'a qu'une seule ligne à rajouter :

```C#
    this.trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);
```

en plein milieu de notre méthode *MakeReservation* de la classe *TicketOffice*; dans le cas où on a effectivement trouvé le bon nombre de place dans le train. Allez, Grand Prince, je vous remet ici toutes la méthode concernée:

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

                // <LINE ADDED HERE TO FIX THE 2ND ACCEPTANCE TEST>
                this.trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);

                return new Reservation(request.TrainId, bookingReference, reservedSeats);
            }
 
            return new Reservation(request.TrainId, string.Empty, reservedSeats);
        }
    }
```

### Et la double-loop bordel ?!?
Aurais-je du faire un test unitaire ici pour faire passer au vert mon 2nd test d'acceptation ? Je vous laisse méditer dessus un instant mais pour moi la réponse fut évidemment non.

Par contre, on ne déconne pas avec le refactoring dans ma famille  (le RED-GREEN-__REFACTORING__ du TDD), celà fut donc ma préoccupation pour les minutes qui ont suivies. Voyons ça ensemble :

### *Mercyless Refactoring* ? Commence par ranger ta chambre !

Les actions de Refactoring ne sont pas toujours glamour mais elles n'en sont pas moins indispensables. Comme le fait de déplacer les différents types générés à la volée (en mode *As if you mean it* si vous vous souvenez bien du dernier épisode), pour les mettre dans les bons projets et répertoires.

Le feignant efficace que je suis s'est bien entendu servi de son IDE et des outils mis à disposition (ici le plugin R#) pour tout faire en un clin d'oeil. Voici comment je m'y prends en général :

1. Je selectionne un projet dans mon explorateur de solution et je __Ctrl-Shift-R__ (*Refactor this!* de R#) pour choisir l'option "*Move types into matching files*". Et hop ! en un instant, toutes les classes et les structs sont réparties dans leur propre fichier au sein du projet en question (pratique pour n'en oublier aucune). J'appelle ça "*déplier*" mon projet.
2. Il ne me reste ensuite plus qu'à leur faire des moves. Là encore une fois, le partisan du moindre effort -pour les tâches stupides- que je suis ne le fait pas fichier par fichier mais sélectionne d'un seul coup dans l'explorateur de solution tous ceux que j'ai envie de bouger dans le même répertoire de destination -> __Ctrl-Shift-R__ à nouveau, pour choisir cette fois le "*Move to Folder*" (F6 pour les intimes) et laisser mon *Richard* préféré faire tous les changements de namespaces qui vont bien (côté déclaration du type mais aussi surtout du côté de tous les endroits qui l'utilisent).

### Certains l'aiment *snob*
Dans certaines conférences, j'ai régulièrement entendu un discours anti-IDE / anti-outil de refactoring pour promouvoir à la place l'usage de la *b... et du couteau*. Quand on voit le temps gagné par exemple sur l'opération de Move décrite précédemment, je dois avouer que je suis perplexe face à cette tendance qui se rapproche d'une forme de snobisme (ou de frime dans certains cas). Ne dit-on pas que *les bons outils font les bons artisans* ? (oui je sais, on dit aussi que *Pierre qui roule n'amasse pas mousse* et que *un vaut mieux que 2 tu l'auras*, mais quand même...)

### Le test, cet incompris
Quand on parle de Refactoring, on parle rarement du refactoring de nos tests, et c'est bien dommage. Parce que des tests fastidieux à lire sont usants à la longue. Ca fatigue tout le monde dans l'équipe, et ça donne des excuses aux professionnels du *ralage* pour vous intoxiquer vos journées (je suis persuadé que vous en avez croisé vous aussi dans vos projets ;-). C'est pour éviter cette situation que j'ai remplacé les 2 lignes suivantes dans mon 1er test d'acceptation :

```C#

var trainDataProvider = Substitute.For<IProvideTrainData>();
trainDataProvider.GetSeats(trainId).Returns(new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) });

```

par une version beaucoup plus expressive et moins fatigante à lire à la longue :

```C#

var trainDataProvider = Substitute.For<IProvideTrainData>();
trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1CoachAnd3SeatsAvailable());

```

avec la petite methode helper qui va bien juste en dessous (méthode static de la classe de Tests d'ici à ce qu'on ai besoin de la réutiliser/déplacer par la suite dans un helper du genre: *TrainTopologyGenerator*) :

```C#

private static List<SeatWithBookingReference> GetTrainWith1CoachAnd3SeatsAvailable()
{
    return new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) };
}

```

J'ai fait la même chose pour le second test d'acceptation mais je ne vous mets ici que le nom de la méthode helper correspondante, parce ça va commencer à devenir un peu chiant comme lecture sinon. Il s'agit de : 

> GetTrainWith1Coach3SeatsIncluding2Available()

Ok. Si on fait un rapide *point route* à ce niveau de notre avancement, on peut dire qu'on a fait un minimum de rangement dans nos projets, puis un tout petit peu de  refatoring (mais dans nos tests d'acceptation cette fois). Il est donc temps de passer à la suite, à savoir : l'écriture de notre 3eme test d'acceptation.

## 3eme test d'acceptation

Pour rappel:
- notre 1er test d'acceptation a montré qu'il était possible d'identifier des sièges adaptés à notre demande de réservation dans le cas trivial d'un train vide.

- notre second test d'acceptation a montré qu'une fois les places identifiées pour une demande de reservation, notre système sollicitait ensuite comme prévu le back-end TrainDataService de l'opérateur historique (manipulé comme un IProvideTrainData par notre logique métier) pour "commiter" la transaction.

### Nos tests, cette dynamique de Design
Jusqu'à maintenant, j'ai surtout utilisé mes tests d'acceptation pour dessiner les contours de mon système considéré depuis l'extérieur comme une grosse boite noire. Celà m'a permis de définir progressivement comment on intéragit avec elle (1er test) et comment elle intéragit avec les services extérieurs (comme ici dans le second test avec les services de  *Hassen Cehef*). __A ce moment de mon avancement, les contours de mon système sont encore grossiers, mais suffisants pour me rassurer__. C'est pour cette raison que j'ai décidé à cet instant là de commencer à "muscler" la logique métier à l'intérieur de ma boîte, en y intégrant cette fois le support de l'invariant métier : "*on ne doit pas remplir un train à plus de 70% de sa capacité totale*". 

### Le moment "*vieux con*"
*Quand j'étais plus jeune...* il m'arrivait de partir bille en tête dans l'écriture d'un test pour lequel le nom (et donc le contrat) n'était pas clair. Dans ces moments là je tatonnais plus ou moins longtemps, jusqu'à ce que j'identifie clairement son objectif (par contre dites pas à ma mère que je faisais comme ça svp). Avec les années, j'ai compris que ce n'étais pas optimal, et ai décidé de me munir d'un petit bout de papier et d'un crayon à côté de moi quand je code, pour pouvoir clarifier certains points et trouver des exemples concrets. Ici, cela m'a servi à trouver l'exemple le plus simple possible pour vérifier cet invariant métier : un train avec une seule voiture de 10 places, dans lequel je ne peux réserver que 7 places. 

> &gt; &gt; DESSIN ICI &lt;&lt;

### Eviter l'effet *magic number*
Autre point important ici : quand on fait du TDD, il est primordial de choisir les cas d'exemples les plus simples possibles pour qu'un nouvel arrivant sur le projet ne puisse pas ensuite s'imaginer en lisant nos tests qu'il y a un cas particulier implicite au delà de ce qu'on teste. En d'autres termes, on veut éviter des situations où le lecteur se dit: "*pourquoi ils ont utilisé le chiffre 3492 ici, est-ce que ce dernier correspond à un cas important pour le métier ?!?*"

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
    var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 10));

    Check.That(reservation.TrainId).IsEqualTo(trainId);
    Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
    Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3), new Seat("A", 4), new Seat("A", 5), new Seat("A", 6), new Seat("A", 7));
}

```

A ce moment du récit, quelque chose devrait vous géner normalement. 

En effet, si on y regarde à 2 fois, le précédent test est un peu stupide. Pourquoi ? allez, je vous laisse 30 secondes pour trouver seul avant de lire la suite.

![jeopardy](https://media.giphy.com/media/xHgVaJ9zySg80/giphy.gif)

Vous voyez le problème maintenant ? C'est en effet le contrat de mon API qui est pourri. Ici, le père de famille nombreuse que je suis demande à reserver 10 places et se voit répondre par le système de réservation: "*on t'en a trouvé et réservé 7*"... Génial, et je fais quoi de mes autres enfants maintenant... En terme d'expérience utilisateur, je crois qu'il n'y a guère que le service de la Poste de mon quartier qui fait mieux (avec leurs avis de passages systématiquement mis dans notre boite à lettre alors qu'on a pris la journée pour les attendre à la maison ;(

### Pair programming
Avec le recul, je suis persuadé que je n'aurai pas pris ce premier exemple un peu couillon si j'avais travaillé *en binôme* avec quelqu'un (pour rappel, j'ai fais ce kata en plusieurs fois -tard le soir- et *on duty*, c.ad. avec un bébé de 2 mois susceptible de m'interrompre à tout moment).

### Le TDD est ton ami
Mais ce n'est pas grave, car la force du TDD est de nous permettre d'avancer sereinement et d'améliorer notre système pas à pas, même en cas de moments d'absence comme ici. Avec le TDD, tous nos pas sont également tracés. On peut donc se libérer très facilement d'une grosse partie de la charge cognitive qu'on a parfois quand on travaille trop longtemps sur un truc qui tient uniquement dans notre tête (*REPL* si tu m'entends...)

Bon. Ce qui est fait est fait, et comme je me sers d'une expérience passée pour rédiger cet article ([voir les détails ici dans le 1er épisode](.RidMeOfThoseTestingPyramids.md)) continuons d'avancer.

RED-__GREEN__-REFACTOR

<w<<>

. On va le voir, on va pouvoir rectifier cette bétîse assez vite par la suite.

### Un enjeu d'apprentissage

---

