//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/bom_line_response.dart';
import 'package:tan_erp_api/src/model/production_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_revision_response.g.dart';

/// BomRevisionResponse
///
/// Properties:
/// * [id] 
/// * [revisionNo] 
/// * [status] 
/// * [outputQuantity] 
/// * [note] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [approvedBy] 
/// * [approvedAtUtc] 
/// * [rowVersion] 
/// * [lines] 
@BuiltValue()
abstract class BomRevisionResponse implements Built<BomRevisionResponse, BomRevisionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'outputQuantity')
  double? get outputQuantity;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'createdBy')
  ProductionPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'approvedBy')
  ProductionPersonResponse? get approvedBy;

  @BuiltValueField(wireName: r'approvedAtUtc')
  DateTime? get approvedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'lines')
  BuiltList<BomLineResponse>? get lines;

  BomRevisionResponse._();

  factory BomRevisionResponse([void updates(BomRevisionResponseBuilder b)]) = _$BomRevisionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomRevisionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomRevisionResponse> get serializer => _$BomRevisionResponseSerializer();
}

class _$BomRevisionResponseSerializer implements PrimitiveSerializer<BomRevisionResponse> {
  @override
  final Iterable<Type> types = const [BomRevisionResponse, _$BomRevisionResponse];

  @override
  final String wireName = r'BomRevisionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProductionPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.approvedBy != null) {
      yield r'approvedBy';
      yield serializers.serialize(
        object.approvedBy,
        specifiedType: const FullType(ProductionPersonResponse),
      );
    }
    if (object.approvedAtUtc != null) {
      yield r'approvedAtUtc';
      yield serializers.serialize(
        object.approvedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomRevisionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNo = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
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
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPersonResponse),
          ) as ProductionPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'approvedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPersonResponse),
          ) as ProductionPersonResponse?;
          if (valueDes == null) continue;
          result.approvedBy.replace(valueDes);
          break;
        case r'approvedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.approvedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineResponse)]),
          ) as BuiltList<BomLineResponse>?;
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
  BomRevisionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomRevisionResponseBuilder();
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


