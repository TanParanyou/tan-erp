//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_document_summary_response.g.dart';

/// QuotationDocumentSummaryResponse
///
/// Properties:
/// * [sha256] 
/// * [byteCount] 
/// * [rendererVersion] 
/// * [renderedAtUtc] 
@BuiltValue()
abstract class QuotationDocumentSummaryResponse implements Built<QuotationDocumentSummaryResponse, QuotationDocumentSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'sha256')
  String? get sha256;

  @BuiltValueField(wireName: r'byteCount')
  int? get byteCount;

  @BuiltValueField(wireName: r'rendererVersion')
  String? get rendererVersion;

  @BuiltValueField(wireName: r'renderedAtUtc')
  DateTime? get renderedAtUtc;

  QuotationDocumentSummaryResponse._();

  factory QuotationDocumentSummaryResponse([void updates(QuotationDocumentSummaryResponseBuilder b)]) = _$QuotationDocumentSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationDocumentSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationDocumentSummaryResponse> get serializer => _$QuotationDocumentSummaryResponseSerializer();
}

class _$QuotationDocumentSummaryResponseSerializer implements PrimitiveSerializer<QuotationDocumentSummaryResponse> {
  @override
  final Iterable<Type> types = const [QuotationDocumentSummaryResponse, _$QuotationDocumentSummaryResponse];

  @override
  final String wireName = r'QuotationDocumentSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationDocumentSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sha256 != null) {
      yield r'sha256';
      yield serializers.serialize(
        object.sha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.byteCount != null) {
      yield r'byteCount';
      yield serializers.serialize(
        object.byteCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.rendererVersion != null) {
      yield r'rendererVersion';
      yield serializers.serialize(
        object.rendererVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.renderedAtUtc != null) {
      yield r'renderedAtUtc';
      yield serializers.serialize(
        object.renderedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationDocumentSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationDocumentSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sha256 = valueDes;
          break;
        case r'byteCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.byteCount = valueDes;
          break;
        case r'rendererVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rendererVersion = valueDes;
          break;
        case r'renderedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.renderedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationDocumentSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationDocumentSummaryResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


