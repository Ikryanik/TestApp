# TestApp

Данный репозиторий содержит решение тестового задания. Это консольное приложение, которое работает с базой данных MS SQL, а именно создает таблицу Customer, заполняет ее, создает индексы и делает выборку данных.

## Запуск приложения из командной строки
* Склонировать репозиторий:
```sh
    git clone https://github.com/Ikryanik/TestApp
    cd .\TestApp
```
Если требуется, можно поменять строку подключения к базе данных в файле TestApp.dll.config

* Чтобы собрать приложение, запустите следующую команду
```sh
      dotnet build .\TestApp.sln
```
* Чтобы запустить приложение и создать таблицу:
```sh
      cd .\TestApp\bin\Debug\net6.0
      TestApp.exe 1
```
Остальные команды описаны ниже
## Доступные команды
* 1 - создание или пересоздание таблицы (в случае, если она уже существует).
* 2 [Фамилия] [Имя] [Отчество] [Дата рождения] [Пол(ж/м)] - создание одной записи.
* 3 - вывод всех строк с уникальным значением ФИО+дата, отсортированным по ФИО, с количеством полных лет.
* 4 - заполнение автоматически 1000100 строк.
* 5 - вывод строк по критерию: пол мужской, ФИО начинается с "F", а также замер скорости выполнения запроса.
* 6 - добавление индексов.

## Результаты выполненной работы
В результате выполнения задачи также решила оптимизировать загрузку данных. Использовала BulkInsert в методе `FillTheTable`. Для примера оставила в классе `DbService` старый метод `FillTheTableBad`.
Замеры показали, что `FillTheTableBad` выполняется в течение **~130000 миллисекунд**, когда метод `FillTheTable` занимает **~9000 миллисекунд**.

Для ускорения выборки данных добавила возможность создания индексов. Это ускоряет выполнение фильтрации почти **в 2 раза**, скриншоты ниже:

До добавления индексов: 

![alt text](https://github.com/Ikryanik/TestApp/blob/master/images/before.png?raw=true)

После добавления индексов:

![alt text](https://github.com/Ikryanik/TestApp/blob/master/images/after.png?raw=true)
