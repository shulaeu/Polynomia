using System.Collections.Generic;
using UnityEngine;

public class Polynomial : MonoBehaviour
{
    [SerializeField] private Transform prefab;
    [SerializeField] private float step;

    private int dataPoints;
    private float xOffset;

    private double[] knownY; // known y points
    private double[] knownX; // know x points
    private double[] coefficientsY; //polynomial coefficients

    private int knownCount;


    private void Start()
    {
        knownCount = transform.childCount;

        if (knownCount <= 1)
        {
            return;
        }

        dataPoints = Mathf.Abs((int)(transform.GetChild(0).position.x - transform.GetChild(knownCount - 1).position.x / step));
        knownY = new double[knownCount];
        knownX = new double[knownCount];

        xOffset = -transform.position.x;

        CalcPolynomial();
    }

    private void CalcPolynomial()
    {
        for (var i = 0; i < knownCount; i++)
        {
            Vector3 currentPoint = transform.GetChild(i).position;
            knownY[i] = currentPoint.y;
            knownX[i] = currentPoint.x + xOffset;
        }

        coefficientsY = CalcCoefficients(knownY);
        for (var i = 0; i < dataPoints; i++)
        {
            double newX = i * knownX[knownCount - 1] / dataPoints;
            double newY = Interpolate(newX);
            var pos = new Vector3((float)newX - xOffset, (float)newY);
            Instantiate(prefab, pos, Quaternion.identity);
        }

    }


    private double[] CalcCoefficients(IReadOnlyList<double> data, int step = 1)
    {

        int count = knownCount;
        var coefficients = new double[count];
        while (true)
        {
            if (count >= 1)
            {
                var calcData = new double[count];
                int i;
                for (i = 0; i < count - 1; i++)
                {
                    calcData[i] = (data[i + 1] - data[i]) / (knownX[i + step] - knownX[i]);
                }

                coefficients[step - 1] = data[0];
                data = calcData;
                count--;
                step++;
                continue;
            }

            break;
        }

        return coefficients;
    }

    private double Interpolate(double dataX)
    {
        int i;
        double yp = 0;
        for (i = 1; i < knownCount; i++)
        {
            double tempYp = coefficientsY[i];
            int k;
            for (k = 0; k < i; k++)
            {
                tempYp *= dataX - knownX[k];
            }

            yp += tempYp;
        }

        return coefficientsY[0] + yp;
    }
}