'LoadAssembly:C:\NetDocumentsDemo\Dll\AutoStoreLibrary.dll

Option Strict Off

Imports System
Imports NSi.AutoStore.Capture.DataModel
Imports AutoStoreLibrary
Imports AutoStoreLibrary.DocuWare
Imports Microsoft.VisualBasic


Module Script
	Sub Form_OnLoad(ByVal eventData As MFPEventData)
		
		Dim orgList As ListField = eventData.Form.Fields.GetField("Org")
		Dim orgs As String
		Dim allOrgs As String()
		Dim org As String
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = "john.campbellhiggens@kofax.com"
		Dim userPassword As String = "Tyl3r5K1ngd0m!"
		'Dim organisation As String = "Kofax UK Ltd"
		
		orgs = DocuWare.GetOrganisations(uri, userName, userPassword)
		allorgs = Split(orgs,"|")
		For Each org In allorgs
			Dim listItem As listItem = New ListItem(org,org)
			orgList.Items.Add(listItem)
		Next
	
	
	End Sub

	Sub Form_OnSubmit(ByVal eventData As MFPEventData)
		'TODO add code here to execute when the user presses OK in the form
	End Sub

	Sub Org_OnChange(ByVal eventData As MFPEventData) 'TODO change <fieldName> to desired field name
		
		
		Dim basketList As ListField = eventData.Form.Fields.GetField("Baskets")
		Dim orgList as ListField = eventData.Form.Fields.GetField("Org")
		
		Dim baskets As String
		Dim allBaskets As String()
		Dim basket As String
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = "john.campbellhiggens@kofax.com"
		Dim userPassword As String = "Tyl3r5K1ngd0m!"
		Dim organisation As String = orgList.Value
		
		baskets = DocuWare.GetBaskets(uri, userName, userPassword, organisation)
		allBaskets = Split(baskets,"|")
		For Each basket In allBaskets
			Dim listItem As listItem = New ListItem(basket,basket)
			basketList.Items.Add(listItem)
		Next
	End Sub

	Sub Baskets_OnChange(ByVal eventData As MFPEventData) 'TODO change <fieldName> to desired field name
	
		Dim basketList As ListField = eventData.Form.Fields.GetField("Baskets")
		Dim orgList As ListField = eventData.Form.Fields.GetField("Org")
		Dim fieldList As TextField = eventData.Form.Fields.GetField("Fields")
		Dim fields As String =""
		
		Dim basket As String = basketList.Value
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = "john.campbellhiggens@kofax.com"
		Dim userPassword As String = "Tyl3r5K1ngd0m!"
		Dim organisation As String = orgList.Value
		
		fields = DocuWare.GetFields(uri, userName, userPassword, organisation, basket)
		fieldList.Value = fields
		
	End Sub
		
	
End Module
