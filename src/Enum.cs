using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcPay
{
    public enum LogisticsType
    {
        /// <summary>
        /// CVS
        /// </summary>
        CVS = 0,
        /// <summary>
        /// Home
        /// </summary>
        Home = 1
    }
    public enum LogisticsSubType
    {
        /// <summary>
        /// TACT
        /// </summary>
        Home_TCAT = 0,
        /// <summary>
        /// ECAN
        /// </summary>
        Home_ECAN = 1
    }

    public enum Temperature
    {
        /// <summary>
        /// 0001
        /// </summary>
        常溫 = 1,
        /// <summary>
        /// 0002
        /// </summary>
        冷藏 = 2,
        /// <summary>
        /// 0003
        /// </summary>
        冷凍 = 3
    }
    public enum Distance
    {
        /// <summary>
        /// 00
        /// </summary>
        同縣市 = 0,
        /// <summary>
        /// 01
        /// </summary>
        外縣市 = 1,
        /// <summary>
        /// 02
        /// </summary>
        離島 = 2
    }
    public enum Specification
    {
        /// <summary>
        /// 0001
        /// </summary>
        _60cm = 1,
        /// <summary>
        /// 0002
        /// </summary>
        _90cm = 2,
        /// <summary>
        /// 0003
        /// </summary>
        _120cm = 3,
        /// <summary>
        /// 0004
        /// </summary>
        _150cm = 4
    }
    public enum ScheduledPickupTime
    {
        /// <summary>
        /// 1
        /// </summary>
        _9_12 = 1,
        /// <summary>
        /// 2
        /// </summary>
        _12_17 = 2,
        /// <summary>
        /// 3
        /// </summary>
        _17_20 = 3,
        /// <summary>
        /// 4
        /// </summary>
        不限時 = 4


    }

    public enum ScheduledDeliveryTime
    {
        /// <summary>
        /// 1
        /// </summary>
        _9_12 = 1,
        /// <summary>
        /// 2
        /// </summary>
        _12_17 = 2,
        /// <summary>
        /// 3
        /// </summary>
        _17_20 = 3,
        /// <summary>
        /// 4
        /// </summary>
        不限時 = 4

    }




}
