//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/bom_line_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_draft_request.g.dart';

/// BomDraftRequest
///
/// Properties:
/// * [outputQuantity] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class BomDraftRequest implements Built<BomDraftRequest, BomDraftRequestBuilder> {
  @BuiltValueField(wireName: r'outputQuantity')
  double? get outputQuantity;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<BomLineRequest>? get lines;

  BomDraftRequest._();

  factory BomDraftRequest([void updates(BomDraftRequestBuilder b)]) = _$BomDraftRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomDraftRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomDraftRequest> get serializer => _$BomDraftRequestSerializer();
}

class _$BomDraftRequestSerializer implements PrimitiveSerializer<BomDraftRequest> {
  @override
  final Iterable<Type> types = const [BomDraftRequest, _$BomDraftRequest];

  @override
  final String wireName = r'BomDraftRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.outputQuantity != null) {
      yield r'outputQuantity';
      yield serializers.serialize(
        object.outputQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomDraftRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'outputQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outputQuantity = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineRequest)]),
          ) as BuiltList<BomLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomDraftRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomDraftRequestBuilder();
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


