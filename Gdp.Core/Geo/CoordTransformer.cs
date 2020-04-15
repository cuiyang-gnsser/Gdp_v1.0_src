//2012.09.23, czs, �޸�, 1.���нǶ����� AngelUnit ����ѡ�Ĭ��Ϊ �ȡ�2.���� XyzToGeoCoord2 ϵ�к�����

using System;
using System.Collections.Generic;
using System.Text; 
using Gdp.Utils;

namespace Gdp
{
    /// <summary>
    /// �ṩ��ݵ�����ת�������� 
    /// </summary>
    public static class CoordTransformer
    {    
        /// <summary>
        ///  �������ǵ�վ�ļ����꣬���ڵ�ǰ��վ�Ĵ�����꣬���߶Ƚǻ���������棬��ǰ����Ϊ0ʱ����ֱ�ӵ��� lon lat
        /// </summary>
        /// <param name="satXyz">���ǵĵ��Ŀռ�ֱ������</param>
        /// <param name="stationPosition">��վ�ĵ��Ŀռ�ֱ������</param>
        /// <param name="unit">�Ƕȵ�λ,��ʾ����ĽǶȵ�λ</param>
        /// <returns></returns>
        public static Polar XyzToGeoPolar(XYZ satXyz, XYZ stationPosition, AngleUnit unit = AngleUnit.Degree)
        {
            var geoCoord = XyzToGeoCoord(stationPosition);          
            return XyzToPolar(satXyz, stationPosition, geoCoord.Lon, geoCoord.Lat, unit);
        } 

        /// <summary>
        /// �������ǵ�վ�ļ�����,ָ���˾�γ�ȣ����Ӿ�ȷ�������ظ����㡣
        /// </summary>
        /// <param name="satXyz"></param>
        /// <param name="stationPosition"></param>
        /// <param name="unit">����ʾ����Ƕȵĵ�λ��ҲҪ������Ƕȵĵ�λ</param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static Polar XyzToPolar(XYZ satXyz, XYZ stationPosition, double lon, double lat, AngleUnit unit = AngleUnit.Degree)
        {
            XYZ deltaXyz = satXyz - stationPosition;//���浽���ǵ��򾶡�

            lon = Math.Abs(lon) < 1E-10 ? 0  : lon;

            NEU neu = XyzToNeu(deltaXyz, lat, lon, unit);
            return NeuToPolar(neu, unit);
        }
        

        /// <summary>
        ///  �ռ�ֱ������ϵ����վ������ϵ��ת����Ĭ��B L ��λ �ȣ�����Ϊ������ʹ������
        ///  ���Ŀռ�ֱ������ϵ(XYZ)ת��Ϊ�ط����ֵѿ���ֱ������ϵ��NEU,XYZ��
        /// </summary>
        /// <param name="vector1">��վ�����ǵ���</param>
        /// <param name="lat">վ������γ��</param>
        /// <param name="lon">վ�����ھ���</param>
        /// <param name="angleUnit">վ�����ھ��ȵĵ�λ</param>
        /// <returns></returns>
        public static NEU XyzToNeu(XYZ vector1, double lat, double lon, AngleUnit angleUnit = AngleUnit.Degree)
        {
            if (angleUnit != AngleUnit.Radian) //��ǰ����Ϊ 0 ��ʱ��U ��ת������ַ��Ŵ��󣿣���������2017.10.12.
            {
                lat = AngularConvert.ToRad(lat, angleUnit);
                lon = AngularConvert.ToRad(lon, angleUnit);
            }
            
            XYZ v = vector1;

            double n = -v.X * Sin(lat) * Cos(lon) - v.Y * Sin(lat) * Sin(lon) + v.Z * Cos(lat);
            double e = -v.X * Sin(lon) + v.Y * Cos(lon);
            double u = v.X * Cos(lat) * Cos(lon) + v.Y * Cos(lat) * Sin(lon) + v.Z * Sin(lat);

            return new NEU(n, e, u);// { N = n, E = e, U = u };
        }
         
        /// <summary>
        /// վ������ϵ��վ�ļ�����ϵ��
        /// </summary>
        /// <param name="neu"></param>
        /// <param name="unit">Ĭ�ϵ�λΪ��</param>
        /// <returns></returns>
        public static Polar NeuToPolar(NEU neu, AngleUnit unit = AngleUnit.Degree)
        {
            double r = neu.Length;
            double a = Math.Atan2(neu.E, neu.N);
            if (a < 0)//�Ա���Ϊ��׼��˳ʱ�룬�޸���
            {
                a += 2.0 * CoordConsts.PI;
            }

            double o = Math.Asin(neu.U / r);
            if (unit != AngleUnit.Radian)
            {
                a = AngularConvert.RadTo(a, unit);
                o = AngularConvert.RadTo(o, unit);
            }
            return new Polar(r, a, o) { Unit = unit };
        }
     

        #region �������Ϳռ�ֱ������֮���ת��

        /// <summary>
        /// �������תΪ�ռ�ֱ�����ꡣ
        /// </summary>
        /// <param name="ellipsoidCoord"></param>
        /// <returns></returns>
        public static XYZ GeoCoordToXyz(GeoCoord ellipsoidCoord, Ellipsoid el = null)
        {
            if (el == null) el = Ellipsoid.WGS84;

            double lon = ellipsoidCoord.Lon;
            double lat = ellipsoidCoord.Lat;
            double height = ellipsoidCoord.Height;
            double a = el.SemiMajorAxis;
            double e = el.FirstEccentricity;

            return GeoCoordToXyz(lon, lat, height, a, e);
        } 

        /// <summary>
        /// ��������תΪ�ռ�ֱ�����ꡣĬ�ϵ�λΪ�ȡ�
        /// </summary>
        /// <param name="lon">���ȣ��ȣ�</param>
        /// <param name="lat">γ�ȣ��ȣ�</param>
        /// <param name="height">��ظ�</param>
        /// <param name="a">����뾶</param>
        /// <param name="flatOrInverse">���ʻ��䵹��</param>
        /// <param name="unit">��λ</param>
        /// <returns></returns>
        public static XYZ GeoCoordToXyz(double lon, double lat, double height, double a, double flatOrInverse, AngleUnit unit = AngleUnit.Degree)
        { 
            lon = AngularConvert.ToRad(lon, unit);
            lat = AngularConvert.ToRad(lat, unit);

            //�����ж�
            double e = flatOrInverse;
            if (flatOrInverse > 1)
            {
                e = 1.0 / e;
            } 

            double n = a / Math.Sqrt(1 - Math.Pow(e * Sin(lat), 2));

            double x = (n + height) * Cos(lat) * Cos(lon);
            double y = (n + height) * Cos(lat) * Sin(lon);
            double z = (n * (1 - e * e) + height) * Sin(lat);
            return new XYZ(x, y, z);
        }

        /// <summary>
        /// �ɿռ�ֱ������ת��Ϊ�������ꡣĬ�ϽǶȵ�λΪ�ȡ�
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="ellipsoid"></param>
        /// <param name="angeUnit"></param>
        /// <returns></returns>
        public static GeoCoord XyzToGeoCoord(IXYZ xyz, Ellipsoid ellipsoid, AngleUnit angeUnit = AngleUnit.Degree)
        {
            double x = xyz.X;
            double y = xyz.Y;
            double z = xyz.Z;

            double a = ellipsoid.SemiMajorAxis;
            double e = ellipsoid.FirstEccentricity;
            return XyzToGeoCoord(x, y, z, a, e, angeUnit);
        }

        /// <summary>
        /// �ɿռ�ֱ������ת��Ϊ������꣬�ο�����ΪWGS84��
        /// Ĭ�ϵ�λΪ�ȡ�
        /// </summary>
        /// <param name="xyz">�ռ�ֱ������</param>
        /// <param name="angeUnit">�Ƕȵ�λ��Ĭ�ϵ�λΪ�ȡ�</param>
        /// <returns></returns>

        public static GeoCoord XyzToGeoCoord(IXYZ xyz, AngleUnit angeUnit = AngleUnit.Degree) { return XyzToGeoCoord(xyz, Ellipsoid.WGS84, angeUnit); }

        /// <summary>
        /// �ɿռ�ֱ������ת��Ϊ�������ꡣĬ�ϽǶȵ�λΪ�ȡ�
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static GeoCoord XyzToGeoCoord(double x, double y, double z, double a, double e, AngleUnit angeUnit = AngleUnit.Degree)
        {
            double ee = e * e;
            double lon = Math.Atan2(y, x);
            double lat;
            double height;

            //iteration
            double deltaZ = 0;
            double sqrtXy = Math.Sqrt(x * x + y * y);
            double tempLat = Math.Atan2(z, sqrtXy);//��ʼȡֵ
            lat = tempLat;
            //if (Math.Abs(lat) > 80.0 * CoordConverter.DegToRadMultiplier)//lat=+-90,��height�ش�ʱ�����ô��㷨���ȶ���ʵ���д�ʵ�飬����both���룩
            //{
            int count = 0;//������ѭ��
            do
            {
                var sinLat = Sin(lat);
                deltaZ = a * ee * sinLat / Math.Sqrt(1 - Math.Pow(e * sinLat, 2));
                tempLat = lat;
                lat = Math.Atan2(z + deltaZ, sqrtXy);
                count++;
            } while (Math.Abs(lat - tempLat) > 1E-12 || count < 20);
            //}
            //else//�����㷨
            //{
            //    do
            //    {
            //        double tanB = Math.Tan(lat);
            //        tempLat = lat;
            //        lat = Math.Atan2(z + a * ee * tanB / Math.Sqrt(1 + tanB * tanB * (1 - ee)), sqrtXy);
            //    } while (Math.Abs(lat - tempLat) > 1E-12);
            //}

            double n = a / Math.Sqrt(1 - Math.Pow(e * Sin(tempLat), 2));
            //double   height = Math.Sqrt(x * x + y * y + Math.Pow((z + deltaZ), 2)) - n;

            height = sqrtXy / Cos(lat) - n;

            //����γ��
            //double dixinLat = (1 - ee * n / (n + height)) * Math.Tan(lat);
            //double dixinLatDeg = dixinLat * CoordConverter.RadToDegdMultiplier;


            lon = AngularConvert.RadTo(lon, angeUnit);
            lat = AngularConvert.RadTo(lat, angeUnit);

            return new GeoCoord(lon, lat, height);
        }
        #endregion
          
        #region ����
        /// <summary>
        ///  Math.Cos(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Cos(double val) { return Math.Cos(val); }
        /// <summary>
        ///  Math.Sin(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Sin(double val) { return Math.Sin(val); }
        /// <summary>
        /// Math.Tan(currentVal)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double Tan(double val) { return Math.Tan(val); }
        #endregion
    }
}
