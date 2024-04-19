using UnityEngine;

namespace AT_RPG
{
    public static class MathfEx
    {
        /// <summary>
        /// Cos의 아랫변의 길이를 계산합니다.
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

        /// <summary>
        /// Sin의 아랫변의 길이를 계산합니다.
        /// </summary>
        /// <param name="topSide">윗변 길이</param>
        /// <param name="angleInDegrees">코사인 각도</param>
        public static float CalculateSinBaseSide(float topSide, float angleInDegrees)
        {
            // 각도를 라디안으로 변환합니다.
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 사인 값을 계산합니다.
            float sinValue = Mathf.Sin(angleInRadians);

            // 아랫변의 길이를 계산합니다.
            float baseSide = topSide * sinValue;

            return baseSide;
        }

        /// <summary>
        /// <paramref name="vector"/>와 가장 방향이 비슷한 기준 벡터를 계산합니다.
        /// </summary>
        public static Vector3 CalculateApproxUnitVector(Vector3 vector, Transform unitTransform)
        {
            Vector3[] units = new Vector3[] { unitTransform.right, -unitTransform.right, unitTransform.forward, -unitTransform.forward, unitTransform.up, -unitTransform.up };

            // 유사도를 계산하기 위한 최대 내적 값 초기화
            float maxDot = float.MinValue;
            Vector3 approx = Vector3.zero;

            // 각 기준 벡터와의 내적 계산
            foreach (Vector3 unit in units)
            {
                float dotProduct = Vector3.Dot(vector.normalized, unit);
                if (dotProduct > maxDot)
                {
                    maxDot = dotProduct;
                    approx = unit;
                }
            }

            // 가장 내적 값이 높은 벡터 반환
            return approx;
        }

        /// <summary>
        /// 콜라이더가 겹치는 영역을 계산합니다.
        /// </summary>
        public static Bounds CalculateIntersectionBounds(Bounds source1, Bounds source2)
        {
            if (!source1.Intersects(source2))
            {
                return new Bounds();
            }

            Vector3 min = Vector3.Max(source1.min, source2.min);
            Vector3 max = Vector3.Min(source1.max, source2.max);
            return new Bounds((min + max) * 0.5f, max - min);
        }

        /// <summary>
        /// <paramref name="unitBasis"/> 방향을 기준으로 충돌하지 않기 위해 필요한 <paramref name="direction"/>과 <paramref name="distance"/>를 계산합니다.
        /// </summary>
        public static bool CalculatePenetration(
            Vector3 unitBasis, Transform unitTransform, Bounds initColliderABounds, Collider colliderA, Collider colliderB, out Vector3 direction, out float distance)
        {
            direction = Vector3.zero;
            distance = 0f;

            Bounds boundsAB = CalculateIntersectionBounds(colliderA.bounds, colliderB.bounds);
            direction = (boundsAB.center - colliderA.bounds.center).normalized;

            switch (unitBasis)
            {
                case Vector3 up when up.Equals(unitTransform.up):
                    break;

                case Vector3 down when down.Equals(-unitTransform.up):
                    break;

                case Vector3 right when right.Equals(unitTransform.right):
                    break;

                case Vector3 left when left.Equals(-unitTransform.right):
                    break;

                case Vector3 forward when forward.Equals(unitTransform.forward):
                    break;

                case Vector3 back when back.Equals(-unitTransform.forward):
                    break;
            }

            return distance > 0f ? true : false;
        }
    }
}