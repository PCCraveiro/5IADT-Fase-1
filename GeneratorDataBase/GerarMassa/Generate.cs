using System.Collections.Concurrent;
using System.Numerics;

namespace GerarMassa;
public class Generate
{
    private Dictionary<FactorsType, int[]> _factorsRange;
    private Dictionary<int, string> _gender;

    public Generate()
    {
        _factorsRange = new Dictionary<FactorsType, int[]>()
        {
            { FactorsType.Sedentary ,new int[] { 90 , 350 } },        // De 9% 35%
            { FactorsType.Smoker ,new int[] { 400 , 950 } },          // De 40% 95%
            { FactorsType.Hypertensive ,new int[] { 100 , 330 } },    // De 10% 33%
            { FactorsType.Diabetic ,new int[] { 70 , 300 } }          // De 7% 30%
        };
        _gender = new Dictionary<int, string>()
        {
            {0, "Masculino" },
            {1, "Feminino" }
        };
    }

    public List<DataDto> GenerateData(int rows)
    {
        var dto = new ConcurrentBag<DataDto>();
        Parallel.For(0, rows, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, i =>
        {
            Random random = new Random();
            var idade = random.Next(0, 95);
            var region = (RegionType)random.Next(1, 5);
            var data = new DataDto()
            {
                Idade = idade,
                Genero = _gender[(random.NextDouble() >= 0.5) ? 1 : 0],
                IMC = GenerateImc(idade),
                Filhos = GenerateChildren(idade),
                Sedentario = GenerateFactor(idade, 5),
                Fumante = GenerateFactor(idade, 13),
                Diabetico = GenerateFactor(idade, -1),
                Hipertenso = GenerateFactor(idade, 7),
                Regiao = region.ToString(),
            };
            data.Encargos = GenerateValueWeightsFor(data);
            dto.Add(data);
        });
        return dto.ToList();
    }

    public decimal GenerateImc(int idade)
    {
        Random random = new Random();
        return (random.Next(15, 40) * ((idade / 100M) + 1));
    }

    public int GenerateChildren(int idade)
    {
        Random random = new Random();

        if (idade <= 13) return 0;

        if (idade <= 15)
        {
            var c = random.Next(0, 3);
            if (c == 3) return 1;
            return 0;
        }

        if (idade <= 18)
        {
            var c = random.Next(0, 5);
            if (c >= 3) return 3;
            return c;
        }

        return random.Next(0, 6);
    }

    public string GenerateFactor(int idade, int limit)
    {
        if (idade <= limit) return "Não";
        Random random = new Random();
        var rd = random.NextDouble() >= 0.5;
        return rd ? "Sim" : "Não";
    }

    public decimal GenerateValueWeightsFor(DataDto dto)
    {
        Random random = new Random();

        // Valor base chutando classes de preços.
        var valueBase = 0M;
        if (dto.Idade <= 5)
            valueBase = Math.Round((random.Next(35555, 88888) / 100M), 2);      // 355,55 a 888,88
        else if (dto.Idade <= 12)
            valueBase = Math.Round((random.Next(55555, 102288) / 100M), 2);     // 555,55 a 1.022,88
        else if (dto.Idade <= 18)
            valueBase = Math.Round((random.Next(68888, 122288) / 100M), 2);     // 688,88 a 1.222,88
        else if (dto.Idade <= 26)
            valueBase = Math.Round((random.Next(78888, 150288) / 100M), 2);     // 788,88 a 1.502,88
        else if (dto.Idade <= 34)
            valueBase = Math.Round((random.Next(98188, 202288) / 100M), 2);     // 981,88 a 2.022,88
        else if (dto.Idade <= 44)
            valueBase = Math.Round((random.Next(108888, 252288) / 100M), 2);    // 1.088,88 a 2.522,88
        else if (dto.Idade <= 44)
            valueBase = Math.Round((random.Next(128888, 302288) / 100M), 2);    // 1.288,88 a 3.022,88
        else if (dto.Idade <= 54)
            valueBase = Math.Round((random.Next(185888, 352288) / 100M), 2);    // 1.588,88 a 3.522,88
        else if (dto.Idade <= 64)
            valueBase = Math.Round((random.Next(180888, 392288) / 100M), 2);    // 1.808,88 a 3.922,88
        else
            valueBase = Math.Round((random.Next(198888, 502288) / 100M), 2);    // 1.988,88 a 5.022,88

        var vlRegion = 0M;  // Sudeste sem desconto.
        switch (dto.Regiao)
        {
            case "Norte":
                vlRegion = (random.Next(30, 500) / 1000M);      // 0,3% a 5%
                break;
            case "Nordeste":
                vlRegion = (random.Next(20, 450) / 1000M);      // 0,2% a 4,5%
                break;
            case "CentroOeste":
                vlRegion = (random.Next(15, 495) / 1000M);      // 0,15% a 4.95%
                break;
            case "Sul":
                vlRegion = (random.Next(10, 299) / 1000M);      // 0,1% a 2,99%
                break;
        }

        var vlSedentary = 1M;
        if (dto.Sedentario == "Sim")
        {
            var f = _factorsRange[FactorsType.Sedentary];
            var minF = f[0];
            var maxF = f[1];
            vlSedentary = (random.Next(minF, maxF) / 1000M) + 1;
        }

        var vlDiabetic = 1M;
        if (dto.Diabetico == "Sim")
        {
            var f = _factorsRange[FactorsType.Diabetic];
            var minF = f[0];
            var maxF = f[1];
            vlDiabetic = (random.Next(minF, maxF) / 1000M) + 1;
        }

        var vlHypertensive = 1M;
        if (dto.Hipertenso == "Sim")
        {
            var f = _factorsRange[FactorsType.Hypertensive];
            var minF = f[0];
            var maxF = f[1];
            vlHypertensive = (random.Next(minF, maxF) / 1000M) + 1;
        }

        var vlSmoker = 1M;
        if (dto.Fumante == "Sim")
        {
            var f = _factorsRange[FactorsType.Smoker];
            var minF = f[0];
            var maxF = f[1];
            vlSmoker = (random.Next(minF, maxF) / 1000M) + 1;
        }

        var vlImc = 1M;
        if (dto.IMC <= 18.5M)
            vlImc = (random.Next(10, 180) / 1000M) + 1;
        else if (dto.IMC >= 25M && dto.IMC < 30M)
            vlImc = (random.Next(10, 180) / 1000M) + 1;
        else if (dto.IMC >= 30M && dto.IMC < 35M)
            vlImc = (random.Next(50, 220) / 1000M) + 1;
        else if (dto.IMC >= 35M && dto.IMC < 40M)
            vlImc = (random.Next(100, 350) / 1000M) + 1;
        else if (dto.IMC >= 40M)
            vlImc = (random.Next(190, 450) / 1000M) + 1;

        var vlChildren = 1M;
        if (dto.Genero == "Feminino")
        {
            if (dto.Filhos >= 1 && dto.Filhos <= 2)
                vlChildren = (random.Next(10, 100) / 1000M) + 1;    // 0,1% a 1%
            else if (dto.Filhos >= 3 && dto.Filhos <= 5)
                vlChildren = (random.Next(50, 150) / 1000M) + 1;    // 0,5% a 1,5%
            else
                vlChildren = (random.Next(50, 190) / 1000M) + 1;    // 0,5% a 1,9%
        }

        var media = (new List<decimal>() { vlSedentary, vlDiabetic, vlHypertensive, vlSmoker, vlImc, vlChildren }).Average();
        var value = valueBase * media;
        return Math.Round(value - (value * vlRegion), 2);
    }
}
