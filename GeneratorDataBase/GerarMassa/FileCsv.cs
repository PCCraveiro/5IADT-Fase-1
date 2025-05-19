using System.Text;

namespace GerarMassa;
public static class FileCsv
{
    public static void GenerateCsv(string filePath, List<DataDto> lst)
    {
        var sb = new StringBuilder();
        sb.AppendLine("idade;genero;imc;filhos;sedentario;fumante;hipertenso;diabetico;regiao;encargos");
        Parallel.ForEach(lst, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, dto =>
        {
            lock (sb)
            {
                sb.AppendLine($"" +
                    $"{dto.Idade};" +
                    $"{dto.Genero};" +
                    $"{dto.IMC};" +
                    $"{dto.Filhos};" +
                    $"{dto.Sedentario};" +
                    $"{dto.Fumante};" +
                    $"{dto.Hipertenso};" +
                    $"{dto.Diabetico};" +
                    $"{dto.Regiao};" +
                    $"{dto.Encargos}");
            }
        });
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }
}
