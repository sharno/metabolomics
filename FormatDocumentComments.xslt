<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">        
	<xsl:template match="sourcefile" mode="seealso-section">
		<h4 class="dtH4">Source File Details</h4>
		<div style="background: #FFFFCC; border: 1px solid #000000; padding: 4px 8px; ">
			This documentation was generated for the
			<b><xsl:value-of select="./project" /> Project</b>
			from the source file
			<b><xsl:value-of select="./filepath" /></b>.
			<br /><br />
			This file was originally created on
			<b><xsl:value-of select="./creation" /></b>
			by
			<xsl:variable name="email">
				<xsl:value-of select="./author/email" />
			</xsl:variable>
			<br /><b><a style="margin-left: 15px;" href="mailto:{$email}"><xsl:value-of select="./author/name" /> (<xsl:value-of select="./author/initials" />)</a></b>
			<br /><br />
			Other contributors include:
			<xsl:apply-templates select="contributors" />
			<br /><br />
			CVS information for this source file:<br />
			<tt style="margin-left: 15px;"><xsl:value-of select="./cvs/cvs_author" /></tt><br />
			<tt style="margin-left: 15px;"><xsl:value-of select="./cvs/cvs_date" /></tt><br />
			<tt style="margin-left: 15px;"><xsl:value-of select="./cvs/cvs_header" /></tt><br />
			<tt style="margin-left: 15px;"><xsl:value-of select="./cvs/cvs_branch" /></tt><br />
			<tt style="margin-left: 15px;"><xsl:value-of select="./cvs/cvs_revision" /></tt><br />
		</div>
	</xsl:template>

	<xsl:template match="contributors">
		<xsl:for-each select="contributor">
			<br />
			<xsl:variable name="cemail">
				<xsl:value-of select="./email" />
			</xsl:variable>
			<a style="margin-left: 15px;" href="mailto:{$cemail}"><xsl:value-of select="./name" /> (<xsl:value-of select="./initials" />)</a>
		</xsl:for-each>
	</xsl:template>

</xsl:stylesheet>