Imports System
Imports System.Collections
Imports System.Drawing
Imports System.Windows.Forms
Imports GTA

    Public Class PedestrianViolence
    Inherits Script
    Private SON As Boolean = False
    Private ZOM As Boolean = False
    Private NITE As Boolean = False
    Private revent As Boolean = False
    Private Group As Group = Player.Group
    Public Sub New()
        Me.interval = 1974
        BindKey(Settings.GetValueKey("Toggle Script", "SETTINGS", Keys.K), New KeyPressDelegate(AddressOf ScriptOn))
        BindKey(Settings.GetValueKey("Toggle Pedestrian Events", "SETTINGS", Keys.L), New KeyPressDelegate(AddressOf RandomEventsPedestrians))
    End Sub
    Private Function RandomNumber(ByVal min As Integer, ByVal max As Integer) As Integer
        Dim random As New Random()
        Return random.Next(min, max)
    End Function
    Private Sub RandomEventsPedestrians()
        If son Then
            revent = Not revent
            If revent Then
                Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Crossfire Starting...", 4000, 1)
            Else
                Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Crossfire Over", 4000, 1)
            End If
        End If
    End Sub
    Private Sub RandomPedEvents_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Tick
        If son Then
            If revent Then
                If Not NITE Then
                    If player.character.isinvehicle = False Then
						Dim random As New Random()
						Dim faction As Integer = RandomNumber(0, 25)
						
						If faction = 5 Then
							RandomEventNOOSE()
						End If
                        
						If faction = 15 Then
							RandomEventGunNuts()
						End If
                    End If
                End If
            End If
        End If
    End Sub
        Private Sub RandomEventNOOSE()
            Dim ped As ped = world.createped("M_Y_SWAT", Player.Character.GetOffsetPosition(New Vector3(30, 30, 0.0F)).toground)
            If exists(ped) AndAlso ped IsNot Nothing Then
                ped.becomemissioncharacter()
                ped.relationshipgroup = relationshipgroup.cop
                ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                ped.ChangeRelationship(relationshipgroup.criminal, relationship.hate)
                ped.task.fightagainsthatedtargets(100.0F)
                ped.maxhealth = 100
                ped.health = 100
                ped.armor = 50
                RandomWeapons(ped)
                'Dim blip As blip = blip.addblip(ped)
                'blip.color = blipcolor.DarkTurquoise
                wait(10)
                ped = world.createped("M_Y_SWAT", Player.Character.GetOffsetPosition(New Vector3(30, 30, 0.0F)).toground)
                If exists(ped) AndAlso ped IsNot Nothing Then
                    ped.becomemissioncharacter()
                    ped.relationshipgroup = relationshipgroup.cop
                    ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                    ped.ChangeRelationship(relationshipgroup.criminal, relationship.hate)
                    ped.task.fightagainsthatedtargets(100.0F)
                    ped.maxhealth = 100
                    ped.health = 100
                    ped.armor = 50
                    RandomWeapons(ped)
                    wait(10)
                End If
                Dim random As New Random()
                Dim n As Integer = RandomNumber(0, 2)
                Dim n2 As Integer = RandomNumber(0, 2)
                If n = 1 Then
                    ped = world.createped("m_m_fbi", Player.Character.GetOffsetPosition(New Vector3(30, 30, 0.0F)).toground)
                    If exists(ped) AndAlso ped IsNot Nothing Then
                        ped.becomemissioncharacter()
                        ped.relationshipgroup = relationshipgroup.cop
                        ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                        ped.ChangeRelationship(relationshipgroup.criminal, relationship.hate)
                        ped.task.fightagainsthatedtargets(100.0F)
                        ped.maxhealth = 100
                        ped.health = 100
                        ped.armor = 50
                        RandomWeapons(ped)
                    End If
                End If

                If n2 = 1 Then
                    wait(10)
                    ped = world.createped("M_Y_SWAT", Player.Character.GetOffsetPosition(New Vector3(30, 30, 0.0F)).toground)
                    If exists(ped) AndAlso ped IsNot Nothing Then
                        ped.becomemissioncharacter()
                        ped.relationshipgroup = relationshipgroup.cop
                        ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                        ped.ChangeRelationship(relationshipgroup.criminal, relationship.hate)
                        ped.task.fightagainsthatedtargets(100.0F)
                        ped.maxhealth = 100
                        ped.health = 100
                        ped.armor = 50
                        RandomWeapons(ped)
                    End If
                End If
            End If
        End Sub
        Private Sub RandomEventGunNuts()
            Dim ped As ped = world.createped("M_M_GunNut_01", Player.Character.GetOffsetPosition(New Vector3(-30, 30, 0.0F)).toground)
            If exists(ped) AndAlso ped IsNot Nothing Then
                ped.becomemissioncharacter()
                ped.relationshipgroup = relationshipgroup.criminal
                ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                ped.task.fightagainsthatedtargets(100.0F)
                ped.maxhealth = 100
                ped.health = 100
                ped.armor = 50
                RandomWeapons(ped)
                'Dim blip As blip = blip.addblip(ped)
                'blip.color = blipcolor.grey
                wait(10)
                ped = world.createped("M_M_GunNut_01", Player.Character.GetOffsetPosition(New Vector3(-30, 30, 0.0F)).toground)
                If exists(ped) AndAlso ped IsNot Nothing Then
                    ped.becomemissioncharacter()
                    ped.relationshipgroup = relationshipgroup.criminal
                    ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                    ped.task.fightagainsthatedtargets(100.0F)
                    ped.maxhealth = 100
                    ped.health = 100
                    ped.armor = 50
                    RandomWeapons(ped)
                    wait(10)
                End If
            End If
            Dim random As New Random()
            Dim gn As Integer = RandomNumber(0, 2)
            Dim gn2 As Integer = RandomNumber(0, 2)
            If gn = 1 Then
                ped = world.createped("M_M_GunNut_01", Player.Character.GetOffsetPosition(New Vector3(-30, 30, 0.0F)).toground)
                If exists(ped) AndAlso ped IsNot Nothing Then
                    ped.becomemissioncharacter()
                    ped.relationshipgroup = relationshipgroup.criminal
                    ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                    ped.task.fightagainsthatedtargets(100.0F)
                    ped.maxhealth = 100
                    ped.health = 100
                    ped.armor = 50
                    RandomWeapons(ped)
                    wait(10)
                End If
            End If
            If gn2 = 1 Then
                ped = world.createped("M_M_GunNut_01", Player.Character.GetOffsetPosition(New Vector3(-30, 30, 0.0F)).toground)
                If exists(ped) AndAlso ped IsNot Nothing Then
                    ped.becomemissioncharacter()
                    ped.relationshipgroup = relationshipgroup.criminal
                    ped.ChangeRelationship(relationshipgroup.dealer, relationship.hate)
                    ped.task.fightagainsthatedtargets(100.0F)
                    ped.maxhealth = 100
                    ped.health = 100
                    ped.armor = 50
                    RandomWeapons(ped)
                End If
            End If
        End Sub
        Private Sub RandomWeapons(ByVal p As ped)
            Dim random As New Random()
            Dim a As Integer = RandomNumber(0, 7)

            If a = 0 Then
                p.Weapons.MP5.Ammo = 999999
            End If

            If a = 1 Then
                p.Weapons.BarettaShotgun.Ammo = 999999
            End If

            If a = 2 Then
                p.Weapons.AssaultRifle_M4.Ammo = 999999
            End If

            If a = 3 Then
                p.Weapons.BasicSniperRifle.Ammo = 999999
            End If

            If a = 4 Then
                p.Weapons.DesertEagle.Ammo = 999999
            End If

            If a = 5 Then
                p.Weapons.BasicShotgun.Ammo = 999999
            End If

            If a = 6 Then
                p.Weapons.AssaultRifle_AK47.Ammo = 999999
            End If

        End Sub
    Private Sub ZombiesAtNight()
        If SON Then
            nite = Not nite
            If nite Then
            Else
            End If
        End If
    End Sub
    Private Sub ZombieChoice()
        If son Then
            ZOM = Not ZOM
            If ZOM Then
            Else
            End If
        End If
    End Sub
        Private Sub ScriptOn()
            SON = Not SON
            If SON Then
            Else
            End If
        End Sub
    End Class