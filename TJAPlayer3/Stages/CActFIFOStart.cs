﻿using FDK;

namespace TJAPlayer3
{
	internal class CActFIFOStart : CActivity
	{
		// メソッド

		public void tフェードアウト開始()
		{
			this.mode = EFIFOモード.フェードアウト;

            this.counter = new CCounter( 0, 750, 1, TJAPlayer3.Timer );
		}
		public void tフェードイン開始()
		{
			this.mode = EFIFOモード.フェードイン;
			this.counter = new CCounter( 0, 750, 1, TJAPlayer3.Timer );
		}

		// CActivity 実装

		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
                //CDTXMania.tテクスチャの解放( ref this.tx幕 );
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FO.png" ) );
 			//	this.tx幕2 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FI.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない || ( this.counter == null ) )
			{
				return 0;
			}
			this.counter.t進行();

			// Size clientSize = CDTXMania.app.Window.ClientSize;	// #23510 2010.10.31 yyagi: delete as of no one use this any longer.

            if( this.mode == EFIFOモード.フェードアウト )
            {
                if( TJAPlayer3.Tx.SongLoading_FadeOut != null )
			    {
					TJAPlayer3.Tx.SongLoading_FadeOut.Opacity = Easing.EaseInOut(counter, 0, 255);
					TJAPlayer3.Tx.SongLoading_FadeOut.t2D描画( TJAPlayer3.app.Device, 0, 0);
                }
			}
            else
            {
                if(TJAPlayer3.Tx.SongLoading_FadeIn != null )
                {
					TJAPlayer3.Tx.SongLoading_FadeIn.Opacity = 255 - Easing.EaseInOut(counter, 0, 255);
					TJAPlayer3.Tx.SongLoading_FadeIn.t2D描画( TJAPlayer3.app.Device, 0, 0 );
                }
            }

            if( this.mode == EFIFOモード.フェードアウト )
            {
			    if( this.counter.n現在の値 != 750 )
			    {
				    return 0;
			    }
            }
            else if( this.mode == EFIFOモード.フェードイン )
            {
			    if( this.counter.n現在の値 != 750 )
			    {
				    return 0;
			    }
            }
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter counter;
        private CCounter ct待機;
		private EFIFOモード mode;
        //private CTexture tx幕;
        //private CTexture tx幕2;
		//-----------------
		#endregion
	}
}
