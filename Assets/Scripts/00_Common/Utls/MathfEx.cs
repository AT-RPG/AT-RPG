using UnityEngine;

namespace AT_RPG
{
    public static class MathfEx
    {
        /// <summary>
        /// 코사인 법칙을 이용해 아랫변의 길이를 계산합니다.
        /// </summary>
        /// <param name="topSide">윗변 길이</param>
        /// <param name="angleInDegrees">코사인 각도</param>
        public static float CalculateCosBaseSide(float topSide, float angleInDegrees)
        {
            // 각도를 라디안으로 변환합니다.
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 코사인 값을 계산합니다.
            float cosineValue = Mathf.Cos(angleInRadians);

            // 아랫변의 길이를 계산합니다.
            float baseSide = topSide * cosineValue;

            return baseSide;
        }

        public static float CalculateSinBaseSide(float topSide, float angleInDegrees)
        {
            // 각도를 라디안으로 변환합니다.
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 사인 값을 계산합니다.
            float cosineValue = Mathf.Sin(angleInRadians);

            // 아랫변의 길이를 계산합니다.
            float baseSide = topSide * cosineValue;

            return baseSide;
        }
    }

}