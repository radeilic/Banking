# Banking


Projektni zadatak **_„Banking Service“_** pred tim je izneo problem Banking Servisa, odnosno servisa koji simulira realni rad banke. Servis simulira i prezentuje odnosno implementira određene realne događaje unutar sistema koji se izvršavaju u realnom vremenu. Servis se sastoji iz pet osnovnih delova: **_BankingService, IDS, AdminApplication, UserApplication_** i **_Commom_**.

Aplikacija implementira autentifikaciju između svih klijenata i **_BankingService_** komponente aplikacije. Autentifikacija se vrši po principu **_SSL_** protokola kao i **_Windows autentifikacionog protokola_**. 

Ono što je specifično za rad **_BankingService_** komponente jeste to što se svaka radnja unutar same komponente loguje u okviru specifičnog log fajla u okviru **_Windows Events Loga_** ali i to što **_BankingService_** poseduje **_„Intrusion Detection System“_** kao i **_„Intrusion Prevention System“_** koji su tu da reaguju i pruže neki vid prevencije odnosno obrane sistema od stranih pretnji.

Sva implementirana sigurnosna ograničenja su konfigurabilna i moguće ih je izmeniti zajedno sa adresama servisa i nazivima korišćenih sertifikata putem **_App.config_** datoteka.







## **Članovi tima:**

[@kbjxkgx Nemanja Ćirić](https://github.com/kbjxkgx)

[@djeka95 Nemanja Đekić](https://github.com/djeka95)

[@RadeIlic Rade Ilić](https://github.com/RadeIlic)

[@KamiSRB Daniel Kamraš](https://github.com/KamiSRB)




## **Dizajn rešenja**
![Dizajn rešenja](https://s8.postimg.org/eh6jjiwk5/dijagram.png)



## **Korišćene klase**
![Koriščene klase](https://s8.postimg.org/oehkclbvp/dijagram2.png)
