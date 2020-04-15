using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{

    

    /// <summary>
    /// �����ꡣ
    /// 
    /// </summary>
    public class PlanePolar//: IPlanePolar// : Coordinate, 
    {
        public PlanePolar()// : base(Referencing.CoordinateReferenceSystem.PlanePolorCrs, 0, Referencing.CoordinateType.RadiusAzimuth)
        { }
        public PlanePolar(double radius, double azimuth, AngleUnit unit = AngleUnit.Degree)
        :this()
        {
            this.Unit = unit;
			this.Range = radius;
			this.Azimuth = azimuth; 
		}
        /// <summary>
        /// �Ƕȵ�λ
        /// </summary>
        public AngleUnit Unit { get;  set; }
	
        /// <summary>
        /// ���� [0, +oo]
        /// </summary>
        public double Range { get; set; }
        /// <summary>
        /// ��λ��[0,360]
        /// </summary>
        public double Azimuth { get; set; } 
        /// <summary>
        /// ��ת��λ��
        /// </summary>
        /// <param name="val"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public PlanePolar RotateAzimuth(double val, AngleUnit unit = AngleUnit.Degree)
        {
            if (this.Unit != unit)
            {
                val = AngularConvert.Convert(val, unit, this.Unit);
            }
            double azimuth = Azimuth + val;
            return new PlanePolar(Range, azimuth);
        }
         

        public override bool Equals(object obj)
        {
            PlanePolar o = obj as PlanePolar;
            if (o == null) return false;


            return Range == o.Range
                && Azimuth == o.Azimuth;
        }

        public override int GetHashCode()
        {
            return Range.GetHashCode() * 3 + Azimuth .GetHashCode() *5 ;
        }



		public override String ToString(){
			return " radius= " + Range + " azimuth= " + Azimuth ; 
		} 
	}

    /// <summary>
    /// �����ꡣ
    /// 
    /// </summary>
    public class Polar //:  IPolar//Coordinate,
    {
        public Polar() //: base(Referencing.CoordinateReferenceSystem.PolorCrs, 0, Referencing.CoordinateType.RadiusAzimuthElevation)
        { }
        public Polar(double radius, double azimuth, double elevatAngle, AngleUnit unit = AngleUnit.Degree)
            : this()
        {
            this.Unit = unit;
            this.Range = radius;
            this.Azimuth = azimuth;
            this.Elevation = elevatAngle;
        }
        public Polar(double radius, double azimuth, AngleUnit unit = AngleUnit.Degree)
            : this()
        {
            this.Unit = unit;
            this.Range = radius;
            this.Azimuth = azimuth;
            this.Elevation = 0;
        }
        /// <summary>
        /// �Ƕȵ�λ
        /// </summary>
        public AngleUnit Unit { get; set; }

        /// <summary>
        /// ���� [0, +oo]
        /// </summary>
        public double Range { get; set; }
        /// <summary>
        /// ˮƽ���� [0, +oo]
        /// </summary>
        public double PlainRange { get { var elevation = AngularConvert.ToRad(Elevation, Unit); return Range * Math.Cos(elevation); } }
        /// <summary>
        /// �߳̾��� [0, +oo]
        /// </summary>
        public double Height { get { var elevation = AngularConvert.ToRad(Elevation, Unit); return Range * Math.Sin(elevation); } }
        /// <summary>
        /// ��λ��[0,360]
        /// </summary>
        public double Azimuth { get; set; }
        /// <summary>
        /// �춥��[0,180]
        /// </summary>
        public double Zenith { get { return 90 - Elevation; } }

        /// <summary>
        /// ��ת��λ��
        /// </summary>
        /// <param name="val"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Polar RotateAzimuth(double val, AngleUnit unit = AngleUnit.Degree)
        {
            if (this.Unit != unit)
            {
                val = AngularConvert.Convert(val, unit, this.Unit);
            }
            double azimuth = Azimuth + val;
            return new Polar(Range, azimuth, Elevation);
        }
        
        /// <summary>
        /// �߶Ƚ� elevationAngle��[-90,90]
        /// </summary>
        public double Elevation { get; set; }

        public override bool Equals(object obj)
        {
            Polar o = obj as Polar;
            if (o == null) return false;


            return Range == o.Range
                && Azimuth == o.Azimuth
                && Elevation == o.Elevation;
        }

        public override int GetHashCode()
        {
            return Range.GetHashCode() * 3 + Azimuth .GetHashCode() *5 + Elevation .GetHashCode();
        }



		public override String ToString(){
			return " radius= " + Range + " azimuth= " + Azimuth + " elevatAngle= " + Elevation; 
		} 
	}
}
