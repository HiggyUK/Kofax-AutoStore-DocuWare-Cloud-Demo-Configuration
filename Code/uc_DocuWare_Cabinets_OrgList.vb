'LoadAssembly:C:\DocuWareCloudDemo\Dll\AutoStoreLibrary.dll

Option Strict Off

Imports System
Imports NSi.AutoStore.Capture.DataModel
Imports AutoStoreLibrary
Imports AutoStoreLibrary.DocuWare
Imports AutoStoreLibrary.Tools
Imports Microsoft.VisualBasic


Module Script
	Sub Form_OnLoad(ByVal eventData As MFPEventData)
		
		' Turn off the Fields not required on first form load
		Dim statusLabel As LabelField = eventData.Form.Fields.GetField("Status")
		eventData.Form.Fields.GetField("Org").IsHidden = True
		eventData.Form.Fields.GetField("Cabinets").IsHidden = True
		statusLabel.IsHidden = True
		
		' Get the current user from the Client - assumes that the user has logged in via Card or PIN
		eventData.Form.Fields.GetField("UserName").Value = eventData.User.UserName
		
	End Sub

	Sub Form_OnSubmit(ByVal eventData As MFPEventData)
		'TODO add code here to execute when the user presses OK in the form
	End Sub

	Sub LoginButton_OnChange(ByVal eventData As MFPEventData)
	
		' Check the Login.  Use the username and password entered
		
		Dim statusLabel As LabelField = eventData.Form.Fields.GetField("Status")
		Dim button As ButtonField = eventData.Form.Fields.GetField(eventData.EventSource)
		Dim orgList As ListField = eventData.Form.Fields.GetField("Org")
		Dim orgs As String
		Dim allOrgs As String()
		Dim org As String
		dim savePassword as String
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = eventData.Form.Fields.GetField("UserName").Value
		Dim userPassword As String = eventData.Form.Fields.GetField("Password").Value
		
		orgs = DocuWare.GetOrganisations(uri, userName, userPassword)
		statusLabel.Text = orgs
		
		If left(orgs,3) = "401" Then
		
			' Login to DocuWare unsuccessful
			
			eventData.Form.StatusMessage = orgs
			statusLabel.Text = orgs
			statusLabel.IsHidden = False
			
		Else
			
			' Login to DocuWare successful
			
			allorgs = Split(orgs,"|")
			For Each org In allorgs
				Dim listItem As listItem = New ListItem(org,org)
				orgList.Items.Add(listItem)
			Next
			
			eventData.Form.Fields.GetField("Org").IsHidden = False
			eventData.Form.Fields.GetField("Cabinets").IsHidden = False
			eventData.Form.Fields.GetField("UserName").IsHidden = True
			eventData.Form.Fields.GetField("Password").IsHidden = True
			eventData.Form.Fields.GetField("LoginButton").IsHidden = True
			eventData.Form.Fields.GetField("Status").IsHidden = True
		
		
			' Save the password to the password cache so it is remembered for next time.
			savePassword = Tools.SavePassword(Tools.EncodeData(userPassword), userName,"C:\NetDocumentsDemo\Passwords")
			If savePassword <> "OK" Then
				statusLabel.Text = savePassword
				statusLabel.IsHidden = False
			End If
			
		End If
		
	End Sub
		
	Sub UserName_OnChange(ByVal eventData As MFPEventData)
		
		' Username has changed, so check if password is already cached
		
		Dim statusLabel As LabelField = eventData.Form.Fields.GetField("Status")
		Dim userName As TextField = eventData.Form.Fields.GetField("UserName")
		Dim userPassword As TextField = eventData.Form.Fields.GetField("Password")

		Dim savePassword As String = ""
		savePassword = Tools.GetPassword(userName.Value,"C:\NetDocumentsDemo\Passwords")
		If left(savePassword,5) = "ERROR" Then
			statusLabel.Text = savePassword
			statusLabel.IsHidden = False
			userPassword.Value = ""
		ElseIf len(savePassword) > 0 Then
			userPassword.Value = Tools.DecodeData(savePassword)
			statusLabel.IsHidden = True
		Else
			userPassword.Value = ""
			statusLabel.IsHidden = True
		End If
		
	End Sub
		
	Sub Org_OnChange(ByVal eventData As MFPEventData) 'TODO change <fieldName> to desired field name
		
		' Organisation selection changed - update the list of File Cabinets
		
		Dim basketList As ListField = eventData.Form.Fields.GetField("Cabinets")
		Dim orgList As ListField = eventData.Form.Fields.GetField("Org")
		Dim statusLabel As LabelField = eventData.Form.Fields.GetField("Status")

		Dim baskets As String
		Dim allBaskets As String()
		Dim basket As String
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = eventData.Form.Fields.GetField("UserName").Value
		Dim userPassword As String = eventData.Form.Fields.GetField("Password").Value
		Dim organisation As String = orgList.Value
		
		baskets = DocuWare.GetFileCabinet(uri, userName, userPassword, organisation)
		allBaskets = Split(baskets,"|")
		For Each basket In allBaskets
			Dim listItem As listItem = New ListItem(basket,basket)
			basketList.Items.Add(listItem)
		Next
	End Sub

	Sub Cabinets_OnChange(ByVal eventData As MFPEventData) 'TODO change <fieldName> to desired field name
	
		' Cabinet selection has changed so, update the list of available fields
		
		Dim statusLabel As LabelField = eventData.Form.Fields.GetField("Status")
		
	
		Dim basketList As ListField = eventData.Form.Fields.GetField("Cabinets")
		Dim orgList As ListField = eventData.Form.Fields.GetField("Org")
		Dim dwFields As String =""
		Dim allDWFields As String()
		Dim fieldSet As String
		Dim fieldValues As String()
		Dim fieldNameDisplay As String
		Dim fieldNameType As Integer
		Dim fieldNameDB As String
		Dim fieldNameScope As Integer
	
		Dim basket As String = basketList.Value
		
		Dim uri As uri = New Uri("https://kofax-demo.docuware.cloud/DocuWare/Platform")
		Dim userName As String = eventData.Form.Fields.GetField("UserName").Value
		Dim userPassword As String = eventData.Form.Fields.GetField("Password").Value
		Dim organisation As String = orgList.Value
		
		'Clear all no config fields
		ResetAllDWFields(eventData)
		
		dwFields = DocuWare.GetFields(uri, userName, userPassword, organisation, basket)
		allDWFields = Split(dwFields,"||")
		For Each fieldSet In allDWFields
			Dim newField As New TextField
			If len(fieldSet) <> 0 And instr(fieldSet,"|") <> 0 Then
				fieldValues = Split(fieldSet,"|")
				fieldNameDB =  fieldValues(0)
				fieldNameDisplay = fieldValues(1)
				fieldNameType = CInt(fieldValues(2))
				fieldNameScope = CInt(fieldValues(3))
		
				' Create new fields
				If len(fieldNameDB) > 0 And fieldNameScope = 0 Then
					newField.Name = "#DW_"+ fieldNameDB
					newField.Display = fieldNameDisplay
					newField.IsHidden = False
					'newField.Value = fieldNameType.ToString
					eventData.Form.Fields.Add(newField)	
				End If
			End If
		Next
		
	End Sub
		
	Sub ResetAllDWFields(ByVal eventData As MFPEventData)

		Dim fieldList As New FieldCollection
		Dim i As Integer
		Dim fields = eventData.Form.Fields
		Dim fieldCount As Integer
		fieldCount = fields.Count() - 1 
			
		For i = 0 To fieldCount
			If left(fields(i).Name,4) = "#DW_"
				fieldList.Add(fields(i))
			End If 
		Next	
		
		For Each field As BaseField In fieldList
			fields.Remove(field)
		Next					
									
						
	End Sub
End Module
