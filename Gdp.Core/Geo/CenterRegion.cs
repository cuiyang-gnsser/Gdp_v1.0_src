//2016.10.28, czs, create in hongqing, ��������

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics; 
using Gdp.Utils;


namespace Gdp
{
    /// <summary>
    /// ��������.��ά����ά 
    public abstract class CenterRegion<TCoord>
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public CenterRegion(TCoord center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        /// <summary>
        /// ��������
        /// </summary>
        public TCoord Center { get; set; }
        /// <summary>
        /// �뾶
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public abstract bool Contains(TCoord coord);
    }

    /// <summary>
    /// ��ά�ռ�ֱ���������������
    /// </summary>
    public class XyzCenterRegion : CenterRegion<XYZ>
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public XyzCenterRegion(XYZ center, double radius) : base(center, radius)
        {
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override bool Contains(XYZ coord)
        {
            return (this.Center - coord).Length <= Radius;
        }
    }
}
