using System;
using System.Data;

namespace KR.MBE.CommonLibrary.Manager
{
    public class QueryManager
    {

        /*

         <?xml version="1.0" encoding="UTF-8"?>
        <message>
            <header>
               <messagename>GetQueryResult</messagename>
                 <replysubject>PROD.KR.ITIER.UI192.168.0.12.TEST</replysubject>
                 <sourcesubject>PROD.KR.ITIER.UI192.168.0.12.TEST</sourcesubject>
                 <targetsubject>PROD.KR.ITIER.TESTsvr</targetsubject>
            </header>
            <body>
               <SITEID>ITIER</SITEID>
               <LANGUAGE>ko</LANGUAGE>
               <QUERYID>TEST1</QUERYID>
               <QUERYVERSION>00001</QUERYVERSION>
            </body>
        </message> 
         */

        public static DataTable getCustomQueryData( String sQueryID, String sQueryVersion, String sLanguage )
        {
            DataTable dtReturn = new DataTable();
            return dtReturn;

        }
    }
}