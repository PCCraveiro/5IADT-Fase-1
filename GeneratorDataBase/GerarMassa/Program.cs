using GerarMassa;

var genetare = new Generate();
var result = genetare.GenerateData(1000000);

var filePath = @"C:\Users\pccra\OneDrive\Estudos\FIAP\5IADT\Trabalhos\DataBase.csv";
FileCsv.GenerateCsv(filePath, result);
