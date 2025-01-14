using AdventOfCodeUtilities;
using System.ComponentModel;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Int64> initialSecrets = inputList.Select(s => Int64.Parse(s)).ToList();

Int64 mix(Int64 secret, Int64 given)
{
    return secret ^ given;
}

Int64 prune(Int64 secret)
{
    return secret & (16777216 - 1);
}

void P1()
{
    Int64 result = 0;
    foreach (Int64 initialSecret in initialSecrets)
    {
        Int64 secret = initialSecret;
        for (int i = 0; i < 2000; i++)
        {

            Int64 step1 = prune(mix(secret, secret << 6));
            Int64 step2 = prune(mix(step1, step1 / 32)); // Integer divison intentional
            Int64 step3 = prune(mix(step2, step2 << 11));

            secret = step3;
        }
        result += secret;
    }


    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;
    List<(List<Int64> secretNumbers, List<int> prices, List<int> changes)> buyersDetails = new();
    foreach (Int64 initialSecret in initialSecrets)
    {
        List<Int64> secretNumbers = new();
        List<int> prices = new();
        List<int> changes = new();
        Int64 secret = initialSecret;
        int price = secret.ToString()[^1] - '0';

        const int length = 2000;
        for (int i = 0; i < length; i++)
        {
            secretNumbers.Add(secret);
            prices.Add(price);

            if (i != length - 1)
            {
                Int64 step1 = prune(mix(secret, secret << 6));
                Int64 step2 = prune(mix(step1, step1 / 32)); // Integer divison intentional
                Int64 step3 = prune(mix(step2, step2 << 11));

                secret = step3;
                int lastPrice = price;
                price = secret.ToString()[^1] - '0';
                changes.Add(price - lastPrice);
            }
        }
        buyersDetails.Add((secretNumbers, prices, changes));
    }

    Dictionary<(int buyerIndex, (int seq1, int seq2, int seq3, int seq4)), int> pricesByBuyerAndSeq = new();

    HashSet<(int seq1, int seq2, int seq3, int seq4)> uniqueSeqs = new();
    for (int buyerIndex = 0; buyerIndex < buyersDetails.Count; buyerIndex++)
    {
        var buyerDetails = buyersDetails[buyerIndex];
        for (int i = 0; i < buyerDetails.changes.Count - 3; i++)
        {
            int seq1 = buyerDetails.changes[i];
            int seq2 = buyerDetails.changes[i + 1];
            int seq3 = buyerDetails.changes[i + 2];
            int seq4 = buyerDetails.changes[i + 3];

            (int seq1, int seq2, int seq3, int seq4) seq = (seq1, seq2, seq3, seq4);
            uniqueSeqs.Add(seq);
            (int buyerIndex, (int seq1, int seq2, int seq3, int seq4)) key = (buyerIndex, seq);
            if (!pricesByBuyerAndSeq.ContainsKey(key))
                pricesByBuyerAndSeq[key] = buyerDetails.prices[i + 4];
        }
    }

    //foreach (var buyerDetails in buyersDetails)
    Int64 maxBananasGained = Int64.MinValue;
    foreach (var seq in uniqueSeqs)
    {
        Int64 bananasGained = 0;

        for (int buyerIndex1 = 0; buyerIndex1 < buyersDetails.Count; buyerIndex1++)
        {
            (int buyerIndex, (int seq1, int seq2, int seq3, int seq4)) key = (buyerIndex1, seq);
            if (pricesByBuyerAndSeq.TryGetValue(key, out int bananasPrice))
            {
                bananasGained += bananasPrice;
            }
        }

        if (bananasGained > maxBananasGained)
            maxBananasGained = bananasGained;
    }
    /*
    HashSet<(int seq1, int seq2, int seq3, int seq4)> seqsTried = new();
    for (int buyerIndex = 0; buyerIndex < buyersDetails.Count; buyerIndex++)
    {
        var buyerDetails = buyersDetails[buyerIndex];
        for (int i = 0; i < buyerDetails.changes.Count - 3; i++)
        {
            int seq1 = buyerDetails.changes[i];
            int seq2 = buyerDetails.changes[i + 1];
            int seq3 = buyerDetails.changes[i + 2];
            int seq4 = buyerDetails.changes[i + 3];
            (int seq1, int seq2, int seq3, int seq4) seq = (seq1, seq2, seq3, seq4);

            if (seqsTried.Add(seq))
            {
                Int64 bananasGained = 0;

                for (int buyerIndex1 = 0; buyerIndex1 < buyersDetails.Count; buyerIndex1++)
                {
                    (int buyerIndex, (int seq1, int seq2, int seq3, int seq4)) key = (buyerIndex1, seq);
                    if (pricesByBuyerAndSeq.TryGetValue(key, out int bananasPrice))
                    {
                        bananasGained += bananasPrice;
                    }
                }

                if (bananasGained > maxBananasGained)
                    maxBananasGained = bananasGained;
            }
        }
    }
    */

    Console.WriteLine(maxBananasGained);
    Console.ReadLine();
}

P1();
P2();
