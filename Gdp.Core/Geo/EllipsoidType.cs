//2014.05.24, czs, created 

using System;
using System.Collections.Generic;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// ���������ͱ�ʶ�����Բ�Ҫ��
    /// </summary>
    public enum EllipsoidType
    {
        /// <summary>
        /// CGCS2000
        /// </summary>
        CGCS2000,
        /// <summary>
        /// WGS84 ����
        /// </summary>
        WGS84,
        /// <summary>
        /// GLONASS ���õ�����
        /// </summary>
        PZ90,
        /// <summary>
        /// ����54����ϵ����
        /// </summary>
        BJ54,
        /// <summary>
        /// ���� 80 ����ϵ
        /// </summary>
        XA80
    }

}