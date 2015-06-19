# Eli_Test_Ylessanne

Server on GUI tarkvara (WPF), mis genereerib ja saadab UDP protokolli kaudu telemeetriat localhost’ile (port 22333) iga ~50ms tagant. Telemeetria paketi sees on üks 4 baidine väärtus, mis näitab olemasolevat nurga väärtust. 

Nurga väärtust kuvamiseks hoitakse decimal arvuna. Paketi saamiseks decimal => int => bytes, nii et vajalikud koma kohad saaksid ka kaasa.Programmi käivitamise ajal on nurk 0. 

Serveri programmi aknas on kaks nuppu, üks nendest vähendab nurka 5 kraadi võrra ja teine suurendab 5 kraadi võrra.

Nurga väärtus väheneb kiirusega 3 kraadi sekundis kuni nullini (suureneb, kui oli negatiivne) ja seda kogu aeg. See tähendab, et kui praegune nurk on 6 kraadi, siis 500ms pärast võrdub see 4.5 kraadiga. Nurga uuendus toimub enne iga paketi saatmist.

Paketi struktuur on:
0x255, b0, b1, b2, b3, Checksum, kus 0x255 on 2 esimest baiti, b0-3 on nurga väärtuse baidid, Checksum on nende baitide summa ühe baidina.

# Client
Client on WPF programm, mis võtab serveri programmist telemeetria pakette vastu, kuvab
antud nurgaga joont ja näitab nurga väärtust teksti kujul

# Nõudmised
Mõlemad programmid on kirjutatud C# keeles ja on ühes Visual Studio solutionis . Lähtekood peab olema GitHub’st kättesaadav ja kompileeritav ilma mingi konfigureerimiseta. Kommentaare lahenduse kohta võib kirjutada kas koodis või eraldi README failis.
Client ’i võrgukood peab olema implementeeritud nii, et ta toetaks osalisi ja vigaseid pakette.
Osalise paketi näide:
Buffer 1: 0x00, 0x255, 0xb0, 0xb1
Buffer 2: 0xb2, 0xb3, Checksum
Vigaseid pakette lihtsalt ignoreerida.
UI thread’i võrgu operatsioonidega blokeerida ei tohi.

# Puudused
Võrgukood on implementeeritud kasutades UdpClient klassi, mis tähendab, et poolikud ja vigased paketid jäetakse kõrvale ning pole oma Udp protokolli loodud. Etteantud paketi struktuur asub paketi saatmisel Udp paketi data väljal.

Teine puudus on, et solutionit käivitades ei tule gitist täielikult konfigureeritud solution kuna osa infot on failides, mis on ignore'itud gitignore poolt ja samas peavad ka olema eemaldatud, et süsteem töötaks. On olemas mingisuguseid workarounde.
