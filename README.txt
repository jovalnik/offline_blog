Detta �r en offlineblog, t�nkt att k�ras lokalt p� anv�ndarens dator.
bloggen n�s p� localhost:8000/top eller
localhost:8000/post (om man vill skapa ny post)

tanken �r att bloggen skall fungera som en vanlig blogg, dvs en kolumn med
inl�gg i sjunkande tidsordning med en lista med etiketter (tags) som man kan
klicka p� om man vill se inl�gg med just den etiketten. Till skillnad fr�n
hur de flesta bloggar funkar �r det dock t�nkt att om man klickar p� en andra
(tredje, nte...) etikett s� visas in�gg som har alla de klickade tagsen. om man g�r tillbaks till
top nollst�lls tagsen.

Detta funkar mer eller mindre nu, bortsett fr�n att tidsordningen p� inl�ggen blir fel efter
andra klicket.

Tyv�rr �r det inte s� mycket annat som funkar som det skall. Programmet krashar om inga
poster med de s�kta etiketterna finns och det har ingen  egentlig CRUD klass - man
kan inte ta bort eller uppdatera poster. Den mesta funktionaliteten sk�ts direkt fr�n 
httpserver.cs, en massa html mallar borde ligga i en egen klass osv, osv.

Jag valde att g�ra detta med vanlig SQL eftersom jag trodde att det skulle bli enklast s� -
jag f�rstod inte riktigt vad jag gjorde i Entity Framework, och jag t�nkte att det skulle g�
snabbare att �teranv�nda gammal kod �n att s�tta sig in i ASP.net, MongoDb etc. Jag hade nog
 fel, inser jag nu.

Jag f�rs�kte anv�nda ett singleton pattern f�r databaskopplingen, men det gjorde att httpserven
krashade. Det har n�got med hur man �ppnar och st�nger anslutningen till databasen att g�ra,
tror jag. Jag f�rst�r inte riktigt detta.

F�rklaring av vad som h�nder n�r en ny post skapas:

Detta sker n�r httplyssnaren - en process som ligger och k�r samtidigt som det �vriga programmet -
uppfattar att browsern aropat adressen newpost/ med metoden POST. programmet l�ser fr�n anropet av
textdata f�r variablerna title tags & body. tags �r p� det h�r stadiet en enda textstr�ng. Nu anropas
databasen. title, tags & body matas in i motsvarande kolumner i tabellen POSTS i databasen. Systemets
nuvarande tid i textstr�ngformat genereras och matas in i kolumnen date. raden med all denna data 
tilldelas automatiskt ett unikt l�pnummer. 

Textstr�ngen tags delas nu upp i  sina best�ndsdelar, ord f�r ord. programmet fr�gar f�r varje ord
om n�gon s�dan tag redan finns i tabellen TAGS. Om inte, l�ggs den till och tilldelas ett l�pnummer
enligt ovan.

Nu g�r programmet igen tagsen igen och fr�gar efter vilket l�pnummer som motsvarar ordet,. detta l�ggs
 i kolumnen tag i tabellen TAGSPOST. Det senasete l�nummret i tabellen POSTS - dvs den nya postens 
l�pnummer l�ggs i kolumnen post i samma rad.

Nu f�ljer en f�rklaring av vad som sker n�r httpservern anropas med en etikett. Programmet fr�gar nu efter
poster i POSTS vars l�pnummer motsvarar v�rdet i kolumnen post i TAGSPOST f�r poster d�r kolumnen tag 
motsvarar l�pnumret f�r poster i tabellen TAGS som har namn som motsvarar etiketten.
dessa l��ggs i en sk data tabell som �r en sorts �terskapade av databasdatan i programminnet. dessa 
datatabeller l�ggs sedan i en lista som inneh�ller reaultatet av eventuella tidigare s�kmningar.

programmet g�r nu igenom tabellen och v�ljer de poster vars l�pnummer f�rekommer i alla datatabeller och
l�gger detta i en datatabell. Fr�n dessa l�ses f�lten titel datum & br�dtext av och l�ggs i en variabel
som matas in i en htmlmall som httpservern serverar till browsern.
 