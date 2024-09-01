using AttractorScheduler.Contracts;
using OfficeOpenXml;

namespace AttractorScheduler.Services;

public class ExcelScheduler : IScheduler
{
    private readonly ExcelPackage _package;
    private ExcelWorksheet? _worksheet;

    public ExcelScheduler(string filePath, string sheetName)
    {
        _package = new ExcelPackage(new FileInfo(filePath));
        SetWorksheet(sheetName);
    }

    private void SetWorksheet(string sheetName)
    {
        _worksheet = _package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase));
        if (_worksheet == null)
            throw new Exception($"Лист для месяца '{sheetName}' не найден.");
    }

    public int FindRowByName(string name)
    {
        for (var row = 1; row <= _worksheet!.Dimension.Rows; row++)
        {
            if (_worksheet.Cells[row, 1].Text.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                return row;
        }
        return -1;
    }

    public void FillCell(int row, int column, double value) => _worksheet!.Cells[row, column].Value = value;

    public int GetColumnForDay(int day, int startColumn = 0)
    {
        for (var column = startColumn; column <= _worksheet!.Dimension.Columns; column++)
        {
            if (int.TryParse(_worksheet.Cells[1, column].Text, out var cellDay) && cellDay == day)
                return column;
        }
        return -1;
    }

    public void Save() => _package.Save();

    public void Dispose() => _package.Dispose();
}