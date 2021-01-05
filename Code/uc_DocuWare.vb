'LoadAssembly:C:\NetDocumentsDemo\Dll\AutoStoreLibrary.dll

Option Strict Off

Imports System
Imports NSi.AutoStore.Capture.DataModel
Imports AutoStoreLibrary
Imports AutoStoreLibrary.DocuWare
Imports Microsoft.VisualBasic


Module Script
	Sub Form_OnLoad(ByVal eventData As MFPEventData)
		
		Dim cabinetList As ListField = eventData.Form.Fields.GetField("Cabinets")
		
		Dim fileCabinets As String
		Dim allCabinets As String()
		Dim cabinet As String
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = "john.campbellhiggens@kofax.com"
		Dim userPassword As String = "Tyl3r5K1ngd0m!"
		Dim organisation As String = "Kofax UK Ltd"
		
		fileCabinets = DocuWare.GetFileCabinet(uri, userName, userPassword, organisation)
		allCabinets = Split(fileCabinets,"|")
		For Each cabinet In allCabinets
			Dim listItem As listItem = New ListItem(cabinet,cabinet)
			cabinetList.Items.Add(listItem)
		Next
		
		eventData.Form.Fields.GetField("Version").Value = AutoStoreLibrary.DocuWare.GetVersion
		eventData.Form.Fields.GetField("Org").Value = AutoStoreLibrary.DocuWare.GetOrganisation(uri, userName, userPassword, organisation)
		
		
	End Sub

    Sub Form_OnSubmit(ByVal eventData As MFPEventData)
        'TODO add code here to execute when the user presses OK in the form
    End Sub

    Sub fieldName_OnChange(ByVal eventData As MFPEventData) 'TODO change <fieldName> to desired field name
        'TODO add code here to execute when field value of <fieldName> is changed
    End Sub

End Module
