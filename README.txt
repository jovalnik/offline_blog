Detta är en offlineblog, tänkt att köras lokalt på användarens dator.
bloggen nås på localhost:8000/top eller
localhost:8000/post (om man vill skapa ny post)

tanken är att bloggen skall fungera som en vanlig blogg, dvs en kolumn med
inlägg i sjunkande tidsordning med en lista med etiketter (tags) som man kan
klicka på om man vill se inlägg med just den etiketten. Till skillnad från
hur de flesta bloggar funkar är det dock tänkt att om man klickar på en andra
(tredje, nte...) etikett så visas inägg som har alla de klickade tagsen. om man går tillbaks till
top nollställs tagsen.

Detta funkar mer eller mindre nu, bortsett från att tidsordningen på inläggen blir fel efter
andra klicket.

Tyvärr är det inte så mycket annat som funkar som det skall. Programmet krashar om inga
poster med de sökta etiketterna finns och det har ingen  egentlig CRUD klass - man
kan inte ta bort eller uppdatera poster. Den mesta funktionaliteten sköts direkt från 
httpserver.cs, en massa html mallar borde ligga i en egen klass osv, osv.

Jag valde att göra detta med vanlig SQL eftersom jag trodde att det skulle bli enklast så -
jag förstod inte riktigt vad jag gjorde i Entity Framework, och jag tänkte att det skulle gå
snabbare att återanvända gammal kod än att sätta sig in i ASP.net, MongoDb etc. Jag hade nog
 fel, inser jag nu.

Jag försökte använda ett singleton pattern för databaskopplingen, men det gjorde att httpserven
krashade. Det har något med hur man öppnar och stänger anslutningen till databasen att göra,
tror jag. Jag förstår inte riktigt detta.

Förklaring av vad som händer när en ny post skapas:

Detta sker när httplyssnaren - en process som ligger och kör samtidigt som det övriga programmet -
uppfattar att browsern aropat adressen newpost/ med metoden POST. programmet läser från anropet av
textdata för variablerna title tags & body. tags är på det här stadiet en enda textsträng. Nu anropas
databasen. title, tags & body matas in i motsvarande kolumner i tabellen POSTS i databasen. Systemets
nuvarande tid i textsträngformat genereras och matas in i kolumnen date. raden med all denna data 
tilldelas automatiskt ett unikt löpnummer. 

Textsträngen tags delas nu upp i  sina beståndsdelar, ord för ord. programmet frågar för varje ord
om någon sådan tag redan finns i tabellen TAGS. Om inte, läggs den till och tilldelas ett löpnummer
enligt ovan.

Nu går programmet igen tagsen igen och frågar efter vilket löpnummer som motsvarar ordet,. detta läggs
 i kolumnen tag i tabellen TAGSPOST. Det senasete lönummret i tabellen POSTS - dvs den nya postens 
löpnummer läggs i kolumnen post i samma rad.

Nu följer en förklaring av vad som sker när httpservern anropas med en etikett. Programmet frågar nu efter
poster i POSTS vars löpnummer motsvarar värdet i kolumnen post i TAGSPOST för poster där kolumnen tag 
motsvarar löpnumret för poster i tabellen TAGS som har namn som motsvarar etiketten.
dessa lääggs i en sk data tabell som är en sorts återskapade av databasdatan i programminnet. dessa 
datatabeller läggs sedan i en lista som innehåller reaultatet av eventuella tidigare sökmningar.

programmet går nu igenom tabellen och väljer de poster vars löpnummer förekommer i alla datatabeller och
lägger detta i en datatabell. Från dessa läses fälten titel datum & brödtext av och läggs i en variabel
som matas in i en htmlmall som httpservern serverar till browsern.
 