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
        public static bool CalculatePenetration(Vector3 unitBasis, Collider colliderA, Collider colliderB, out Vector3 direction, out float distance)
        {
            direction = Vector3.zero;
            distance = 0f;

            // unitBasis를 colliderB의 로컬 축에 맞게 변환
            Vector3 localUnitBasis = colliderB.transform.InverseTransformDirection(unitBasis).normalized;

            // 두 Bounds를 각 객체의 Transform을 반영하여 계산
            Bounds boundsA = TransformBounds(colliderA.bounds, colliderA.transform);
            Bounds boundsB = TransformBounds(colliderB.bounds, colliderB.transform);

            // 두 Bounds의 충돌 중심과 크기 계산
            Bounds intersection = CalculateIntersectionBounds(boundsA, boundsB);
            Vector3 intersectionSize = intersection.size;
            Vector3 centerDifference = (boundsA.center - intersection.center).normalized;

            // 로컬 축을 기준으로 충돌 방향과 거리 설정
            direction = GetMajorAxis(localUnitBasis);
            distance = GetPenetrationDepth(intersectionSize, localUnitBasis);

            if (distance > 0)
            {
                direction = colliderB.transform.TransformDirection(direction); // 월드 공간으로 방향 변환
                return true;
            }

            return false;
        }

        private static Bounds TransformBounds(Bounds bounds, Transform transform)
        {
            // 중심점을 Transform의 위치, 회전, 스케일에 따라 변환
            Vector3 transformedCenter = transform.TransformPoint(bounds.center);

            // 각 축의 extents를 월드 스케일로 변환하여 새로운 extents 계산
            Vector3 transformedExtents = Vector3.zero;
            transformedExtents.x = transform.TransformVector(Vector3.right * bounds.extents.x).magnitude;
            transformedExtents.y = transform.TransformVector(Vector3.up * bounds.extents.y).magnitude;
            transformedExtents.z = transform.TransformVector(Vector3.forward * bounds.extents.z).magnitude;

            // 변환된 중심점과 extents로 새로운 Bounds 생성
            return new Bounds(transformedCenter, transformedExtents * 2);
        }

        // 주 축을 추출하는 함수
        private static Vector3 GetMajorAxis(Vector3 vector)
        {
            float absX = Mathf.Abs(vector.x);
            float absY = Mathf.Abs(vector.y);
            float absZ = Mathf.Abs(vector.z);

            if (absX > absY && absX > absZ)
                return vector.x > 0 ? Vector3.right : Vector3.left;
            else if (absY > absZ)
                return vector.y > 0 ? Vector3.up : Vector3.down;
            else
                return vector.z > 0 ? Vector3.forward : Vector3.back;
        }

        // 충돌 깊이를 계산하는 함수
        private static float GetPenetrationDepth(Vector3 intersectionSize, Vector3 localUnitBasis)
        {
            if (Mathf.Approximately(localUnitBasis.x, 1) || Mathf.Approximately(localUnitBasis.x, -1))
                return intersectionSize.x;
            else if (Mathf.Approximately(localUnitBasis.y, 1) || Mathf.Approximately(localUnitBasis.y, -1))
                return intersectionSize.y;
            else if (Mathf.Approximately(localUnitBasis.z, 1) || Mathf.Approximately(localUnitBasis.z, -1))
                return intersectionSize.z;
            return 0;
        }
    }

}