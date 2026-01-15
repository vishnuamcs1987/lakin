<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="top">
    <xsl:if test="name(row[1]/*[1]) = 'ITEM'">
      <BODY>
        <xsl:for-each select="row">
          <OBJECT>
            <InventoryRecord xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
              <!-- <ExtensionData /> -->
              <!-- <Vehicle> -->
              <xsl:copy-of select="*" />
              <!-- </Vehicle>			 -->
            </InventoryRecord>
          </OBJECT>				
        </xsl:for-each>
      </BODY>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>