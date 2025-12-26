<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>
	<xsl:template match="top">
		<xsl:if test="row/*[name() = 'Route_ID']">
			<BODY>
				<xsl:for-each select="row">
					<OBJECT>
						<MasterRouteInformationResult xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
							<ExtensionData />
							<MasterRoute>
								<xsl:copy-of select="*" />
							</MasterRoute>			
						</MasterRouteInformationResult>
					</OBJECT>				
				</xsl:for-each>
			</BODY>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>