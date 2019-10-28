# Signal shower
Spustitelná ukázka se nachází ve složce Runnable. Po spuštění začne aplikace okamžitě fungovat a vykreslovat sinusovu křivku. Křivka sinu je použita jako defaultní pro generátor signálu a dá se změnit klepnutím na jednu z možností v levé horní části aplikace.

## O kódu
Aplikace je rozdělená do dvou části. První část posílá hodnoty na port a druhá část je zobrazuje uživateli do okna a nechá uživatele měnit druh generovaného signálu. První část se nachází v souboru Generators.cs a druhá ve Form1.cs. 

Část na generování signálu je navržena tak, aby se dali jednotlivé funkce signálu jednoduše: upravovat, přidávat nové, měnit jim parametry. Možnost měnění parametrů je jedno z možných rozšíření aplikace. Celá třída funguje naprosto stejně bez ohledu na to, jaký signál je vybrán. Do funkce pro výpočet pošle data a výsledek pošle na port.

Část na příjem dat je navržena tak, aby četla jen z jednoho zdroje a neumí přepínat zdroj vstupních dat. Očekává, že data budou ve správném formátu. Kdyby náhodou nebyli, tak data zahodí. Ty, které budou ve špatném formátu, ale budou mít alespoň očekávanou délku, tak jsou vypsány do konzole (při ladění programu).s
