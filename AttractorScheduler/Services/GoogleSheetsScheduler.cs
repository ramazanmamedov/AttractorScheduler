using AttractorScheduler.Contracts;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace AttractorScheduler.Services;

public class GoogleSheetsScheduler : IScheduler
{
    private readonly string _spreadsheetId;
    private readonly SheetsService _service;
    private readonly string _sheetName;
    private readonly IList<IList<object>> _sheetData;
    private readonly ILogger _logger;

    public GoogleSheetsScheduler(string credentialsPath, string spreadsheetId, string sheetName, ILogger logger)
    {
        _spreadsheetId = spreadsheetId;
        _sheetName = sheetName;
        _logger = logger;

        UserCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { SheetsService.Scope.Spreadsheets },
                "user",
                CancellationToken.None,
                new FileDataStore("GoogleSheetsTokenStore", true)).Result;
        }

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Sheets Scheduler"
        });

        var range = $"{_sheetName}!1:151";
        var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
        var response = request.Execute();
        _sheetData = response.Values;
    }

    public int FindRowByName(string nameOrSurname)
    {
        for (var row = 0; row < _sheetData?.Count; row++)
        {
            if (_sheetData[row].Count > 0 && _sheetData[row][0].ToString()!.IndexOf(nameOrSurname, StringComparison.OrdinalIgnoreCase) >= 0)
                return row;
        }
        return -1;
    }

    public void FillCell(int row, int column, double value)
    {
        column++; //не знаю почему, но google sheets берет предыдущую колонку, не смог разобраться
        var range = $"{_sheetName}!{GetColumnName(column)}{row + 2}";
        var valueRange = new ValueRange { Values = new List<IList<object>> { new List<object> { value } } };

        var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        updateRequest.Execute();
    }

    public int GetColumnForDay(int day, int startColumn = 0)
    {
        for (var column = startColumn; column < _sheetData[0].Count; column++)
        {
            if (int.TryParse(_sheetData[0][column].ToString(), out var cellDay))
            {
                if (cellDay == day)
                    return column;
            }
        }
        _logger.LogError($"Колонка для дня {day} не найдена");
        return -1;
    }
    public void Save()
    {
        // Ignore
    }

    public void Dispose()
    {
        // Ignore
    }

    private static string GetColumnName(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var columnName = "";

        while (index > 0)
        {
            var remainder = (index - 1) % 26;
            columnName = letters[remainder] + columnName;
            index = (index - remainder) / 26;
        }

        return columnName;
    }
}