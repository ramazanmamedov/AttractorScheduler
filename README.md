# Excel/Google Sheets Scheduler

## Описание

Этот проект представляет собой приложение на C#, которое автоматически заполняет расписание преподавателей в зависимости от их занятий и вебинаров. Приложение поддерживает работу как с локальными Excel-файлами, так и с удаленными таблицами Google Sheets.
Программа автоматически заполнит расписание в выбранном источнике данных (Excel или Google Sheets).

## Функциональность

- Заполнение расписания занятий и вебинаров для преподавателя.
- Поддержка как локальных Excel-файлов, так и Google Sheets.
- Гибкость настроек через файл конфигурации `appsettings.json`.
- Возможность ввода частичного имени или фамилии для поиска преподавателя.
- Поддержка Dependency Injection для выбора реализации (Excel или Google Sheets).

- Если вы планируете использовать Google Sheets:
	•	Создайте проект в Google Cloud Console.
	•	Включите Google Sheets API.
	•	Создайте учетные данные OAuth 2.0 и скачайте файл credentials.json.
	•	Поместите файл credentials.json в корневую папку вашего проекта.


## Архитектура

Проект организован по принципам SOLID и использует Dependency Injection для гибкого выбора реализации источника данных (Excel или Google Sheets). Основные компоненты проекта:

	•	IExcelScheduler — интерфейс для работы с различными источниками данных.
	•	ExcelScheduler — реализация для работы с локальными Excel-файлами.
	•	GoogleSheetsScheduler — реализация для работы с Google Sheets.
	•	TeacherSchedule — класс, ответственный за заполнение расписания.
	•	DependencyResolver — простой DI-контейнер для управления зависимостями.
	•	AppConfig — класс для загрузки и управления конфигурацией приложения.

Примеры

Пример конфигурации appsettings.json:
{
  "UseGoogleSheets": true,
  "ExcelFilePath": "C:\\path\\to\\your\\file.xlsx",
  "ClassDurationHours": 2,
  "WebinarDurationHours": 2,
  "WebinarPreparationHours": 1,
  "ClassPreparationRowOffset": 1,
  "WebinarRowOffset": 2,
  "WebinarPreparationRowOffset": 3
  "GoogleCredentials" : "credentials.json",
  "SpreadsheetId" : "SpreadsheetId",
  "TeacherName" : "Иван Иванов",
  "ClassDays" : "Вторник, Пятница",
  "WebinarDays" : "Среда"
}
