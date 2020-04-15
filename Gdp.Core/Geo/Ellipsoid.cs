//2014.05.24, czs, created 
//2019.01.09, czs, edit in hmx, ����CGCS2000, WGS84 �����ᣬ���ʳ���

using System;
using System.Collections.Generic;
using System.Text;


namespace Gdp
{

    /// <summary>
    /// �����塣
    /// </summary>
    public class Ellipsoid : IdentifiedObject//, IEllipsoid
    {

        #region Constructors
        /// <summary>
        /// Ĭ�Ϲ��캯����ʲô��û�С�
        /// </summary>
        public Ellipsoid() { }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="semiMajorAxis">������</param>
        /// <param name="flatteningOrInverse">���ʻ��䵹�����������ݴ�С�����Զ��ж�</param>
        /// <param name="axisUnit">�����������λ</param>
        public Ellipsoid(double semiMajorAxis, double flatteningOrInverse, LinearUnit axisUnit = null, string name = null)
        {
            //default meter
            this.AxisUnit = axisUnit == null ? LinearUnit.Metre : axisUnit;
            this.Name = name == null ? "δ��������" : name;

            this.SemiMajorAxis = semiMajorAxis;

            if (flatteningOrInverse < 1)//С�� 1 ���Ǳ��ʣ����� 1 �����䵹��
            {
                this.Flattening = flatteningOrInverse;
                this.InverseFlattening = 1 / flatteningOrInverse;
            }
            else
            {
                this.Flattening = 1 / flatteningOrInverse;
                this.InverseFlattening = flatteningOrInverse;
            }

            this.SemiMinorAxis = semiMajorAxis * (1 - Flattening);
            this.PolarCurvatureSemiAxis = semiMajorAxis * semiMajorAxis / SemiMinorAxis;
            this.FirstEccentricity = Math.Sqrt(semiMajorAxis * semiMajorAxis - SemiMinorAxis * SemiMinorAxis) / semiMajorAxis;
            this.SecondEccentricity = Math.Sqrt(semiMajorAxis * semiMajorAxis - SemiMinorAxis * SemiMinorAxis) / SemiMinorAxis;
        }

        /// <summary>
        /// �ɳ�����ͱ�����̰���
        /// </summary>
        /// <param name="semiMajorAxis"></param>
        /// <param name="flattening"></param>
        /// <returns></returns>
        private static double GetSemiMinorAxis(double semiMajorAxis, double flattening)
        {
            return semiMajorAxis * (1 - flattening);
        }
        /// <summary>
        /// ����ʡ� e = (a-b)/a
        /// </summary>
        /// <param name="semiMajorAxis"></param>
        /// <param name="semiMinorAxis"></param>
        /// <returns></returns>
        private static double GetFlattening(double semiMajorAxis, double semiMinorAxis)
        {
            return (semiMajorAxis - semiMinorAxis) / semiMajorAxis;
        }

        /// <summary>
        /// ���ع�ϣ���롣
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return SemiMajorAxis.GetHashCode() * 13 + SemiMinorAxis.GetHashCode() * 5;
        }

        /// <summary>
        /// ����ֵ���ж��Ƿ���ȡ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Ellipsoid))
                return false;
            Ellipsoid e = obj as Ellipsoid;
            return (e.InverseFlattening == this.InverseFlattening &&
                    e.SemiMajorAxis == this.SemiMajorAxis &&
                    e.SemiMinorAxis == this.SemiMinorAxis &&
                    e.AxisUnit.Equals(this.AxisUnit));
        }


        #endregion

        #region property

        /// <summary>
        /// ���ȵ�λ
        /// </summary>
        public LinearUnit AxisUnit { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public double SemiMajorAxis { get; set; }
        /// <summary>
        /// �̰���
        /// </summary>
        public double SemiMinorAxis { get; set; }

        /// <summary>
        /// ���ʵĵ���
        /// </summary>
        public double InverseFlattening { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public double Flattening { get; set; }
        /// <summary>
        /// �����ʰ뾶
        /// </summary>
        public double PolarCurvatureSemiAxis { get; set; }
        /// <summary>
        /// ��һƫ����
        /// </summary>
        public double FirstEccentricity { get; set; }
        /// <summary>
        /// �ڶ�ƫ����
        /// </summary>
        public double SecondEccentricity { get; set; }
        /// <summary>
        /// gravitational constant
        /// </summary>
        public double GM { get; set; }
        /// <summary>
        /// earth angular velocity (rad)
        /// </summary>
        public double AngleVelocity { get; set; }
        /// <summary>
        /// ����ƽ���뾶  6371000
        /// </summary>
        public const double MeanRaduis = 6371000;
        #endregion
        /// <summary>
        /// �ַ���
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region ��������
        /// <summary>
        /// WGS84 ���� ������ 6378137
        /// </summary>
        public const double SemiMajorAxisOfWGS84 = 6378137;
        /// <summary>
        ///  WGS84 ���� ���ʵ��� 298.257223563
        /// </summary>
        public const double InverseFlatOfWGS84 = 298.257223563;
        /// <summary>
        /// CGCS2000 ���� ������ 6378137.0
        /// </summary>
        public const double SemiMajorAxisOfCGCS2000 = 6378137.0;
        /// <summary>
        /// CGCS2000 ���� ���ʵ��� 298.257222101
        /// </summary>
        public const double InverseFlatOfCGCS2000 = 298.257222101;
        /// <summary>
        /// CGCS 2000���й�2000���Ҵ������ϵ���õ�����
        /// ��ITRF97�ο����Ϊ��׼���ο���ԪΪ2000.
        /// </summary>
        public static Ellipsoid CGCS2000 => new Ellipsoid(6378137.0, 298.257222101)
                {
                    Abbreviation = "CGCS2000",
                    Name = "2000�й��������ϵ",
                    Id = "",
                    GM = 3.986004418E14,
                    AngleVelocity = 7.292115E-5

                };
        /// <summary>
        /// Glonass
        /// </summary>
        public static Ellipsoid PZ90 => new Ellipsoid(6378136.0, 298.257)
                {
                    Abbreviation = "PZ90",
                    Name = "PZ90",
                    Id = "",
                    GM = 3.9860044E14,
                    AngleVelocity = 7.292115E-5
                };
        /// <summary>
        /// ����54����ϵ
        /// </summary>
        public static Ellipsoid BeiJing54 => new Ellipsoid(6378245.0, 298.3)
                {
                    Abbreviation = "BJ54",
                    Name = "����54����"
                };
        /// <summary>
        /// ���� 80 ����ϵ
        /// </summary>
        public static Ellipsoid XiAn80 => new Ellipsoid(6378140.0, 298.257)
                {
                    Abbreviation = "XiAn80",
                    Name = "����80����"
                };

        /// <summary>
        /// WGS 84 ellipsoid
        /// </summary>
        /// <remarks>
        /// Inverse flattening derived from four defining parameters 
        /// (semi-major axis;
        /// C20 = -484.16685*10e-6;
        /// earth's angular velocity w = 7292115e11 rad/sec;
        /// gravitational constant GM = 3986005e8 m*m*m/s/s).
        /// </remarks>
        public static Ellipsoid WGS84 => new Ellipsoid(6378137, 298.257223563)
                {
                    Id = "7030",
                    Name = "WGS 84",
                    Abbreviation = "WGS84",
                    GM = 3.9860050E14,
                    AngleVelocity = 7.2921151467E-5
                };
        /// <summary>
        /// WGS 72 Ellipsoid
        /// </summary>
        public static Ellipsoid WGS72 => new Ellipsoid(6378135.0, 298.26)
                {
                    Id = "7043",
                    Name = "WGS 72",
                    Abbreviation = "WGS72"
                };


        /// <summary>
        /// GRS 1980 / International 1979 ellipsoid
        /// </summary>
        /// <remarks>
        /// Adopted by IUGG 1979 Canberra.
        /// Inverse flattening is derived from
        /// geocentric gravitational constant GM = 3986005e8 m*m*m/s/s;
        /// dynamic form factor J2 = 108263e8 and Earth's angular velocity = 7292115e-11 rad/s.")
        /// </remarks>
        public static Ellipsoid GRS80 => new Ellipsoid(6378137, 298.257222101)
                {
                    Id = "7019",
                    Name = "International 19792",
                    Abbreviation = "GRS 1980"
                };

        /// <summary>
        /// International 1924 / Hayford 1909 ellipsoid
        /// </summary>
        /// <remarks>
        /// Described as a=6378388 m. and b=6356909m. from which 1/f derived to be 296.95926. 
        /// The figure was adopted as the International ellipsoid in 1924 but with 1/f taken as
        /// 297 exactly from which b is derived as 6356911.946m.
        /// </remarks>
        public static Ellipsoid International1924 => new Ellipsoid(6378388, 297)
                {
                    Id = "7022",
                    Name = "International 1924",
                    Abbreviation = "Hayford 1909"
                };

        /// <summary>
        /// Clarke 1880
        /// </summary>
        /// <remarks>
        /// Clarke gave a and b and also 1/f=293.465 (to 3 decimal places).  1/f derived from a and b = 293.4663077
        /// </remarks>
        public static Ellipsoid Clarke1880 => new Ellipsoid(20926202, 297)
                {
                    Id = "7034",
                    Name = "Clarke 1880",
                    Abbreviation = "Clarke 1880",
                    AxisUnit = LinearUnit.ClarkesFoot
                };
        /// <summary>
        /// Clarke 1866
        /// </summary>
        /// <remarks>
        /// Original definition a=20926062 and b=20855121 (British) feet. Uses Clarke's 1865 inch-metre ratio of 39.370432 to obtain metres. (Metric value then converted to US survey feet for use in the United States using 39.37 exactly giving a=20925832.16 ft US).
        /// </remarks>
        public static Ellipsoid Clarke1866 => new Ellipsoid(6378206.4, 1.0 / GetFlattening(6378206.4, 6356583.8))
                 {
                     Id = "7008",
                     Name = "Clarke 1866",
                     Abbreviation = "Clarke 1866",
                     AxisUnit = LinearUnit.Metre
                 };

        /// <summary>
        /// Sphere
        /// </summary>
        /// <remarks>
        /// Authalic sphere derived from GRS 1980 ellipsoid (code 7019).  (An authalic sphere is
        /// one with a surface area equal to the surface area of the ellipsoid). 1/f is infinite.
        /// </remarks>
        public static Ellipsoid Sphere => new Ellipsoid(6370997.0, double.PositiveInfinity)
                {
                    Id = "7048",
                    Name = "GRS 1980 Authalic Sphere",
                    Abbreviation = "Sphere",
                    AxisUnit = LinearUnit.Metre
                };
        #endregion

        /// <summary>
        /// �������ͣ���ȡָ��������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Ellipsoid GetEllipsoid(EllipsoidType type)
        {

            switch (type)
            {
                case EllipsoidType.CGCS2000:
                    return Ellipsoid.CGCS2000;
                case EllipsoidType.BJ54:
                    return Ellipsoid.BeiJing54;
                case EllipsoidType.WGS84:
                    return Ellipsoid.WGS84;
                case EllipsoidType.PZ90:
                    return Ellipsoid.PZ90;
                case EllipsoidType.XA80:
                    return Ellipsoid.XiAn80;
                default:
                    return Ellipsoid.WGS84;
            }
        }
        /// <summary>
        /// �������ͣ���ȡָ��������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Ellipsoid> GetEllipsoids()
        {
            var els = new List<Ellipsoid>();

            els.Add(Ellipsoid.CGCS2000);
            els.Add(Ellipsoid.WGS84);
            els.Add(Ellipsoid.XiAn80);
            els.Add(Ellipsoid.PZ90);
            els.Add(Ellipsoid.BeiJing54);
            return els;
        }

    }
}