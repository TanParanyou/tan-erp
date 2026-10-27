package com.tanerp.tan_erp_field.ar

import android.content.Context
import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.Path
import android.graphics.PointF
import android.graphics.RectF
import android.util.TypedValue
import android.view.MotionEvent
import android.view.View

/** Text placed at an edge's on-screen midpoint. */
class EdgeLabel(val text: String, val at: PointF)

/** Everything the overlay draws for one camera frame; built on the GL thread. */
class OverlayState(
    /** Screen position per placed point; null when the point is behind the camera. */
    val points: List<PointF?>,
    val closed: Boolean,
    val labels: List<EdgeLabel>,
    val reticleOnPlane: Boolean,
)

/**
 * Transparent view above the camera: reticle, points, dimension lines and
 * per-edge lengths. All corners are square (Atelier, radius 0); all colours come
 * from the Flutter request.
 */
class MeasureOverlayView(
    context: Context,
    private val colors: ArMeasureColors,
    private val onTap: (Float, Float) -> Unit,
) : View(context) {
    var state: OverlayState? = null

    private val density = resources.displayMetrics.density
    private val pointHalf = 5f * density
    private val reticleHalf = 14f * density
    private val labelPad = 4f * density

    private val linePaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        style = Paint.Style.STROKE
        strokeWidth = 2f * density
        color = colors.accentMeasure.toInt()
    }
    private val fillPaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        style = Paint.Style.FILL
        // Same oak accent, translucent, so the measured floor stays visible.
        color = colors.accentMeasure.toInt()
        alpha = FILL_ALPHA
    }
    private val pointPaint = Paint().apply {
        style = Paint.Style.FILL
        color = colors.accentMeasure.toInt()
    }
    private val pointBorderPaint = Paint().apply {
        style = Paint.Style.STROKE
        strokeWidth = density
        color = colors.onAction.toInt()
    }
    private val reticlePaint = Paint().apply {
        style = Paint.Style.STROKE
        strokeWidth = 2f * density
    }
    private val labelBgPaint = Paint().apply {
        style = Paint.Style.FILL
        color = colors.surface.toInt()
    }
    private val labelTextPaint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        color = colors.textMain.toInt()
        textSize = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_SP, LABEL_SP, resources.displayMetrics)
        fontFeatureSettings = "tnum"
        textAlign = Paint.Align.CENTER
    }

    override fun onTouchEvent(event: MotionEvent): Boolean {
        if (event.action == MotionEvent.ACTION_UP) {
            onTap(event.x, event.y)
            performClick()
        }
        return true
    }

    override fun performClick(): Boolean = super.performClick()

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        val s = state ?: return
        drawReticle(canvas, s.reticleOnPlane)

        val visible = s.points
        if (s.closed && visible.size >= 3 && visible.all { it != null }) {
            val path = Path()
            visible.forEachIndexed { i, p -> if (i == 0) path.moveTo(p!!.x, p.y) else path.lineTo(p!!.x, p.y) }
            path.close()
            canvas.drawPath(path, fillPaint)
        }
        val edgeCount = if (s.closed) visible.size else visible.size - 1
        for (i in 0 until edgeCount.coerceAtLeast(0)) {
            val a = visible[i] ?: continue
            val b = visible[(i + 1) % visible.size] ?: continue
            canvas.drawLine(a.x, a.y, b.x, b.y, linePaint)
        }
        for (p in visible) {
            p ?: continue
            canvas.drawRect(p.x - pointHalf, p.y - pointHalf, p.x + pointHalf, p.y + pointHalf, pointPaint)
            canvas.drawRect(p.x - pointHalf, p.y - pointHalf, p.x + pointHalf, p.y + pointHalf, pointBorderPaint)
        }
        val metrics = labelTextPaint.fontMetrics
        for (label in s.labels) {
            val w = labelTextPaint.measureText(label.text) / 2f + labelPad
            val h = (metrics.descent - metrics.ascent) / 2f + labelPad
            canvas.drawRect(RectF(label.at.x - w, label.at.y - h, label.at.x + w, label.at.y + h), labelBgPaint)
            canvas.drawText(label.text, label.at.x, label.at.y - (metrics.ascent + metrics.descent) / 2f, labelTextPaint)
        }
    }

    private fun drawReticle(canvas: Canvas, onPlane: Boolean) {
        val cx = width / 2f
        val cy = height / 2f
        reticlePaint.color = (if (onPlane) colors.onAction else colors.textMuted).toInt()
        canvas.drawRect(cx - reticleHalf, cy - reticleHalf, cx + reticleHalf, cy + reticleHalf, reticlePaint)
        canvas.drawLine(cx - pointHalf, cy, cx + pointHalf, cy, reticlePaint)
        canvas.drawLine(cx, cy - pointHalf, cx, cy + pointHalf, reticlePaint)
    }

    private companion object {
        const val LABEL_SP = 13f
        const val FILL_ALPHA = 0x40
    }
}
