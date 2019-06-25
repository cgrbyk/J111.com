SELECT U.UrunID,U.UrunAdi,U.UrunFiyat,U.UrunStok,U.UrunAciklama,U.UrunKategoriID,K.KategoriAdi,SUBSTRING( 
( 
     SELECT ',' + OzellikAdı AS 'data()' 
         FROM OZELLIKLER
		 where UrunID=100001
		 FOR XML PATH('') 
), 2 , 9999) As OzelliklerAd
,SUBSTRING( 
( 
     SELECT ',' + OzellikAciklama AS 'data()' 
         FROM OZELLIKLER
		 where UrunID=100001
		 FOR XML PATH('') 
), 2 , 9999) As OzellikAciklama
FROM URUNLER as U , KATEGORILER as K 
WHERE U.UrunKategoriID=K.KategoriId AND U.UrunId=100001