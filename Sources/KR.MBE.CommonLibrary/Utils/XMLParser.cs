using System.Collections.Generic;
using System.Text;

/// 
namespace KR.MBE.CommonLibrary.Utils
{
    // XML �ļ� ���ؼ� XML���� �ִ� ���� �����´�.  
    public class XMLParser
    {
        public static string[] convertToStringArray( IList<string> pList )
        {
            string[] returnObject = null;

            if( pList != null )
            {

                string strLog = "List has {} items." + pList.Count.ToString();

                returnObject = new string[pList.Count];
                int counter = 0;
                for( IEnumerator<string> it = pList.GetEnumerator(); it.MoveNext(); )
                {
                    returnObject[counter] = it.Current;
                    counter++;
                }
            }
            else
            {
                string err = "List was null";
            }
            return ( returnObject );

        }


        // Comment ���� 
        public virtual void removeComments( StringBuilder pageContents )
        {
            string startComment = "<!--";
            string endComment = "-->";

            while( pageContents.ToString().IndexOf( startComment ) != -1 )
            {
                int startIndex = pageContents.ToString().IndexOf( startComment );
                int endIndex = pageContents.ToString().IndexOf( endComment ) + endComment.Length;
                pageContents.Remove( startIndex, endIndex );
            }
        }

        // tab ĳ���� ���ڿ� ���� 	 
        public static StringBuilder removeTabChars( StringBuilder pXML )
        {
            return ( removeCharacter( pXML, '\t' ) );
        }

        // Line ĳ���� ���ڿ� ����  
        public static StringBuilder removeNewlines( StringBuilder pXML )
        {
            return ( removeCharacter( pXML, '\n' ) );
        }

        // ������ ���ڿ� �Է¹޾Ƽ� �ش� ���ڿ� ����   
        private static StringBuilder removeCharacter( StringBuilder pXML, char pChar )
        {
            StringBuilder returnXML = new StringBuilder();
            for( int i = 0; i < pXML.Length; i++ )
            {
                if( pXML[i] != pChar )
                {
                    returnXML.Append( pXML[i] );
                }
            }
            return ( returnXML );

        }


        // xml ������ �ִ� ���� �±װ��� �����´�. 
        public static string getValueForTag( StringBuilder pString, string pTag )
        {

            string startTag = "<" + pTag;
            string endTag = "</" + pTag + ">";

            string returnVal = null;
            int startIndex = pString.ToString().IndexOf( startTag );
            if( startIndex != -1 )
            {

                // strLog = "xml ���ڿ��� �±� ã��" ; 
                int endStartTag = pString.ToString().IndexOf( ">", startIndex );

                startTag = pString.ToString().Substring( startIndex, endStartTag + 1 - startIndex );

                if( startTag.Replace( " ", "" ) == "<" + pTag + "/>" )
                {
                    returnVal = "";
                }
                else
                {
                    string fullTag = getEntireTag( pString, startIndex, endTag, startTag );

                    returnVal = fullTag.Substring( startTag.Length, fullTag.Length - endTag.Length - startTag.Length );
                    returnVal = ( removeTabChars( new StringBuilder( returnVal ) ) ).ToString();
                }
            }
            else
            {
                string errMsg = "xml ���ڿ������� �±׸� ã���� �����ϴ�.";
            }
            return ( returnVal );
        }


        //  ��ü XML �±׸� �����´� 
        // pStartIndex : ã���� �ϴ� XML �±��� ��ġ 
        // pEndTag : ������ �±� 
        // pStartTag : �����±� 
        public static string getEntireTag( StringBuilder pXML, int pStartIndex, string pEndTag, string pStartTag )
        {

            int startVal = pStartIndex + pStartTag.Length;
            int endStartVal = pStartIndex + pStartTag.Length;
            string tag = null;

            int possEndIndex = -1;
            int newStartIndex = -1;
            do
            {
                possEndIndex = pXML.ToString().IndexOf( pEndTag, endStartVal );
                newStartIndex = pXML.ToString().IndexOf( pStartTag, startVal );
                endStartVal = possEndIndex + pEndTag.Length;
                startVal = newStartIndex + pStartTag.Length;
            } while( ( newStartIndex != -1 ) && ( possEndIndex > newStartIndex ) );

            tag = pXML.ToString().Substring( pStartIndex, possEndIndex + pEndTag.Length - pStartIndex );
            return ( tag );
        }


        // ����,���� �±� ������ ��ü XML �±װ��� �����´�. 
        public static string getEntireTag( StringBuilder pXML, int pStartIndex, string pStartTag )
        {


            StringBuilder tempEndTag = new StringBuilder( pStartTag.Trim() );
            tempEndTag.Insert( 1, "/" );
            string endTag = tempEndTag.ToString();

            return ( getEntireTag( pXML, pStartIndex, endTag, pStartTag ) );
        }


        // XML ���ڿ� ������ �ش� �±װ��� �迭�� �����´� 
        // pTag : ã���� �ϴ� tag�� 
        // �±װ� �迭 ���� 
        public static string[] getMatchingNestedTags( string pXmlString, string pTag )
        {

            IList<string> nestedVals = getNestedTags( pXmlString );
            string startTag = "<" + pTag + ">";

            IList<string> returnVals = new List<string>();
            foreach( string tag in nestedVals )
            {
                if( tag.StartsWith( startTag ) )
                {
                    returnVals.Add( tag );
                }
            }
            return convertToStringArray( returnVals );
        }

        // XML ���ڿ� top-level���� �ִ� ��� �±װ� ArrayList �迭 ����  
        private static IList<string> getNestedTags( string pXmlString )
        {

            StringBuilder xmlContents = removeNewlines( new StringBuilder( pXmlString ) );
            IList<string> nestedVals = new List<string>();

            while( xmlContents.Length > 0 )
            {
                int nextClosing = xmlContents.ToString().IndexOf( ">" );
                if( nextClosing == -1 )
                {
                    xmlContents.Remove( 0, xmlContents.Length );
                }
                else
                {
                    string thisStartTag = xmlContents.ToString().Substring( 0, nextClosing + 1 );
                    string firstEntry = getEntireTag( xmlContents, 0, thisStartTag );
                    xmlContents.Remove( 0, firstEntry.Length );
                    nestedVals.Add( firstEntry );
                }
            }

            return ( nestedVals );
        }

        // XML ���ڿ� Top-Level ���� �ִ� ��� Nested tag ����  
        // �±װ� ���ڿ� �迭 ���� 
        public static string[] getAllNestedTags( string pXmlString )
        {
            IList<string> nestedTags = getNestedTags( pXmlString );
            return convertToStringArray( nestedTags );

        }
    }
}