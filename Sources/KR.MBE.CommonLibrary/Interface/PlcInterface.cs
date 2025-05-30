 using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR.MBE.CommonLibrary.Struct;

namespace KR.MBE.CommonLibrary.Interface
{
    /// <summary>
    /// PLC Interface
    /// 이기종간의 PLC 통신을 위한 Interface 정의 (실제 PLC 드라이버에서 이 인터페이스 클레스를 추상 상속하여 구현해야한다)
    /// </summary>
    public interface PlcInterface
    {
        /// <summary>
        /// PLC 드라이버에 있는 테그 리스트 가져오기
        /// </summary>
        /// <returns>TagBaseList 타입으로 반환</returns>
        public TagBaseList GetTagList();

        /// <summary>
        /// PLC 드라이버 시작
        /// </summary>
        public void CommStart();

        /// <summary>
        /// PLC 드라이버 종료
        /// </summary>
        public void CommEnd();

        /// <summary>
        /// PLC 드라이버에 크레인 정보 반영
        /// </summary>
        /// <param name="dr">DB 데이터 정보 (DataRow)</param>
        public void SetStation(DataRow dr);

        /// <summary>
        /// PLC 드라이버로 태그 쓰기
        /// </summary>
        /// <param name="dr">DB 데이터 정보 (DataRow)</param>
        /// <returns>성공 여부 (BOOL)true/false</returns>
        public bool WriteTag(DataRow dr);
        
        /// <summary>
        /// PLC 드라이버 태그 쓰기
        /// </summary>
        /// <param name="dt">DB 데이터 정보 (DataTable)</param>
        /// <returns>성공 여부 (문자열)SUCCESS/FAILED</returns>
        public string WriteTag(DataTable dt);

        /// <summary>
        /// PLC 드라이버 태그 쓰기
        /// </summary>
        /// <param name="tagId">태그 ID</param>
        /// <param name="value">쓸 값</param>
        /// <returns>성공 여부 (BOOL)true/false</returns>
        public bool WriteTag(string tagId, string value);

        public string EQUIPMENTID { get; set; }

        public string PROTOCOLNAME { get; set; }
        public bool commStatus { get; }
        public bool commEnable { get; set; }

        public string EquipmentType { get; set; }

        public string STATIONID { get; set; }
    }
}
